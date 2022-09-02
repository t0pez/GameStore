using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Events.GameKeyUpdated;
using GameStore.Core.Exceptions;
using GameStore.Core.Extensions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.Loggers;
using GameStore.Core.Interfaces.RelationshipModelsServices;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Mongo.Products;
using GameStore.Core.Models.Mongo.Products.Specifications;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.Games.Specifications;
using GameStore.Core.Models.Server.RelationalModels;
using GameStore.Core.Models.Server.RelationalModels.Specifications;
using GameStore.Core.Models.ServiceModels.Enums;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MediatR;
using MongoDB.Bson;

namespace GameStore.Core.Services;

public class GameService : IGameService
{
    private static readonly DateTime AddedToStoreAtForMongo = new(2022, 6, 2);
    private readonly IRelationshipModelService<GameGenre> _gameGenreService;
    private readonly IRelationshipModelService<GamePlatformType> _gamePlatformService;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IMongoLogger _mongoLogger;
    private readonly ISearchService _searchService;
    private readonly IUnitOfWork _unitOfWork;

    public GameService(ISearchService searchService,
                       IRelationshipModelService<GameGenre> gameGenreService,
                       IRelationshipModelService<GamePlatformType> gamePlatformService,
                       IMediator mediator, IMongoLogger mongoLogger, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _searchService = searchService;
        _gameGenreService = gameGenreService;
        _gamePlatformService = gamePlatformService;
        _mediator = mediator;
        _mongoLogger = mongoLogger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private IRepository<Game> GameRepository => _unitOfWork.GetEfRepository<Game>();

    private IRepository<Product> ProductsRepository => _unitOfWork.GetMongoRepository<Product>();

    public async Task<Game> CreateAsync(GameCreateModel model)
    {
        var game = await CreateGameAsync(model, DateTime.UtcNow);

        await _mongoLogger.LogCreateAsync(game);

        return game;
    }

    public async Task<int> GetTotalCountAsync()
    {
        var games = await GameRepository.GetBySpecAsync(new GamesSpec());
        var products = await ProductsRepository.GetBySpecAsync();

        var mappedGames = _mapper.Map<IEnumerable<ProductDto>>(games);
        var mappedProducts = _mapper.Map<IEnumerable<ProductDto>>(products);
        var mappedItems = mappedGames.Concat(mappedProducts);

        mappedItems = mappedItems.DistinctBy(item => item.Key);

        return mappedItems.Count();
    }

    public async Task<Game> GetByKeyAsync(string gameKey)
    {
        var spec = new GamesSpec().ByKey(gameKey).WithDetails();

        var result = await GameRepository.GetSingleOrDefaultBySpecAsync(spec)
                     ?? throw new ItemNotFoundException(typeof(Game), gameKey);

        return result;
    }

    public async Task<Game> GetByIdAsync(Guid id)
    {
        var spec = new GamesSpec().ById(id);

        var result = await GameRepository.GetSingleOrDefaultBySpecAsync(spec)
                     ?? throw new ItemNotFoundException(typeof(Game), id);

        return result;
    }

    public async Task UpdateAsync(GameUpdateModel updateModel)
    {
        switch (updateModel.Database)
        {
            case Database.Server:
                await UpdateServerEntityAsync(updateModel);

                break;
            case Database.Mongo:
                await UpdateMongoEntityAsync(updateModel);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task UpdateFromEndpointAsync(GameUpdateModel updateModel)
    {
        switch (updateModel.Database)
        {
            case Database.Server:
                await UpdateServerEntityAsync(updateModel);

                break;
            case Database.Mongo:
                await MigrateMongoEntityAsync(updateModel);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task DeleteAsync(string gameKey, Database database)
    {
        var game = await _searchService.GetProductDtoByGameKeyOrDefaultAsync(gameKey)
                   ?? throw new ItemNotFoundException(typeof(Game), gameKey);

        game.IsDeleted = true;
        var updateModel = _mapper.Map<GameUpdateModel>(game);

        switch (database)
        {
            case Database.Server:
                await UpdateServerEntityAsync(updateModel);

                break;
            case Database.Mongo:
                await MigrateMongoEntityAsync(updateModel);

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(database), database, null);
        }
    }

    public async Task<byte[]> GetFileAsync(string gameKey)
    {
        var spec = new GamesSpec().ByKey(gameKey);

        var game = await GameRepository.GetSingleOrDefaultBySpecAsync(spec)
                   ?? throw new ItemNotFoundException(typeof(Game), gameKey);

        return game.File;
    }

    public async Task<bool> IsGameKeyAlreadyExists(string gameKey)
    {
        var gameSpec = new GamesSpec().ByKey(gameKey).LoadAll();
        var productSpec = new ProductsSpec().ByGameKey(gameKey);

        return await GameRepository.AnyAsync(gameSpec) || await ProductsRepository.AnyAsync(productSpec);
    }

    private async Task MigrateMongoEntityAsync(GameUpdateModel updateModel)
    {
        var product = await _searchService.GetProductDtoByGameKeyOrDefaultAsync(updateModel.OldGameKey);

        var oldVersionOfProduct = product.ToBsonDocument();

        var createModel = _mapper.Map<GameCreateModel>(product);

        var game = _mapper.Map<Game>(createModel);
        game.AddedToStoreAt = AddedToStoreAtForMongo;

        await UpdateGameValues(game, updateModel);

        updateModel.Id = product.Id;
        await UpdateMongoEntityAsync(updateModel);

        await GameRepository.AddAsync(game);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogUpdateAsync(typeof(Game), oldVersionOfProduct, game.ToBsonDocument());
    }

    private async Task UpdateServerEntityAsync(GameUpdateModel updateModel)
    {
        var gameKey = updateModel.OldGameKey ?? updateModel.Key;
        var spec = new GamesSpec().ByKey(gameKey).WithDetails();

        var game = await GameRepository.GetSingleOrDefaultBySpecAsync(spec)
                   ?? throw new ItemNotFoundException(typeof(Game), gameKey);

        updateModel.Id = game.Id.ToString();
        var oldVersionOfGame = game.ToBsonDocument();

        await UpdateGameValues(game, updateModel);

        await _mongoLogger.LogUpdateAsync(typeof(Game), oldVersionOfGame, game.ToBsonDocument());

        await GameRepository.UpdateAsync(game);
        await _unitOfWork.SaveChangesAsync();

        await EnsureUpdateNotificationPublishedAsync(updateModel);
    }

    private async Task UpdateMongoEntityAsync(GameUpdateModel updateModel)
    {
        var product = _mapper.Map<Product>(updateModel);

        await ProductsRepository.UpdateAsync(product);

        await EnsureUpdateNotificationPublishedAsync(updateModel);
    }

    private async Task EnsureUpdateNotificationPublishedAsync(GameUpdateModel updateModel)
    {
        if (updateModel.IsGameKeyChanged)
        {
            await _mediator.Publish(new GameKeyUpdatedEvent(updateModel.OldGameKey, updateModel.Key));
        }
    }

    private async Task<Game> CreateGameAsync(GameCreateModel model, DateTime addedToStore)
    {
        var game = _mapper.Map<Game>(model);
        game.AddedToStoreAt = addedToStore;

        await GameRepository.AddAsync(game);
        await _unitOfWork.SaveChangesAsync();

        return game;
    }

    private async Task UpdateGameValues(Game game, GameUpdateModel updateModel)
    {
        await UpdateGameRelationshipModelsAsync(updateModel);

        game.Name = updateModel.Name;
        game.Key = updateModel.Key;
        game.Description = updateModel.Description;
        game.Price = updateModel.Price;
        game.File = updateModel.File;
        game.PublisherName = updateModel.PublisherName;
        game.Views = updateModel.Views;

        if (updateModel.IsDeleted.HasValue)
        {
            game.IsDeleted = updateModel.IsDeleted.Value;
        }
    }

    private async Task UpdateGameRelationshipModelsAsync(GameUpdateModel updateModel)
    {
        var newGameGenres =
            updateModel.GenresIds.Select(id => new GameGenre { GameId = Guid.Parse(updateModel.Id), GenreId = id });

        var newGamePlatforms =
            updateModel.PlatformsIds.Select(id => new GamePlatformType
                                                { GameId = Guid.Parse(updateModel.Id), PlatformId = id });

        await _gameGenreService.UpdateManyToManyAsync(newGameGenres,
                                                      new GameGenresByGameIdSpec(Guid.Parse(updateModel.Id)));

        await _gamePlatformService.UpdateManyToManyAsync(newGamePlatforms,
                                                         new GamePlatformsByPlatformIdSpec(Guid.Parse(updateModel.Id)));
    }
}