using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Events.Notifications;
using GameStore.Core.Exceptions;
using GameStore.Core.Extensions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.Loggers;
using GameStore.Core.Interfaces.RelationshipModelsServices;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.Core.Models.Games.Specifications.Filters;
using GameStore.Core.Models.Genres;
using GameStore.Core.Models.Genres.Specifications;
using GameStore.Core.Models.Mongo.Products;
using GameStore.Core.Models.PlatformTypes;
using GameStore.Core.Models.PlatformTypes.Specifications;
using GameStore.Core.Models.RelationalModels;
using GameStore.Core.Models.RelationalModels.Specifications;
using GameStore.Core.Models.ServiceModels.Enums;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.Core.PagedResult;
using GameStore.SharedKernel.Interfaces.DataAccess;
using GameStore.SharedKernel.Specifications.Extensions;
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
    private IRepository<Genre> GenreRepository => _unitOfWork.GetEfRepository<Genre>();
    private IRepository<PlatformType> PlatformTypesRepository => _unitOfWork.GetEfRepository<PlatformType>();
    private IRepository<Product> ProductsRepository => _unitOfWork.GetMongoRepository<Product>();

    public async Task<Game> CreateAsync(GameCreateModel model)
    {
        var game = await CreateGameAsync(model, DateTime.UtcNow);

        await _mongoLogger.LogCreateAsync(game);

        return game;
    }

    public async Task<int> GetTotalCountAsync()
    {
        var games = await GameRepository.GetBySpecAsync(new GamesListSpec());
        var products = await ProductsRepository.GetBySpecAsync();

        var mappedGames = _mapper.Map<IEnumerable<ProductDto>>(games);
        var mappedProducts = _mapper.Map<IEnumerable<ProductDto>>(products);
        var mappedItems = mappedGames.Concat(mappedProducts);

        mappedItems = mappedItems.DistinctBy(item => item.Key);

        return mappedItems.Count();
    }

    public async Task<PagedResult<Game>> GetByFilterAsync(GameSearchFilter filter)
    {
        if (filter.GenresIds.Any())
        {
            var genresWithChildren = await GetGenresWithChildrenAsync(filter.GenresIds);
            filter.GenresIds = genresWithChildren;
        }

        var games = await GameRepository.GetBySpecAsync(new GamesByFilterSpec(filter).EnablePaging(filter));

        var result = new PagedResult<Game>(games, totalGamesCount, filter);

        return result;
    }

    public async Task<Game> GetByKeyAsync(string gameKey)
    {
        var result = await GameRepository.GetSingleOrDefaultBySpecAsync(new GameByKeyWithDetailsSpec(gameKey))
                     ?? throw new ItemNotFoundException(typeof(Game), gameKey);

        return result;
    }

    public async Task<Game> GetByIdAsync(Guid id)
    {
        var result = await GameRepository.GetSingleOrDefaultBySpecAsync(new GameByIdSpec(id))
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
        var game = await GameRepository.GetSingleOrDefaultBySpecAsync(new GameByKeySpec(gameKey))
                   ?? throw new ItemNotFoundException(typeof(Game), gameKey);

        return game.File;
    }

    public async Task<bool> IsGameKeyAlreadyExists(string gameKey)
    {
        return await GameRepository.AnyAsync(new GameByKeySpec(gameKey).IncludeDeleted());
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
        var game = await GameRepository.GetSingleOrDefaultBySpecAsync(
                       new GameByKeyWithDetailsSpec(updateModel.OldGameKey ?? updateModel.Key))
                   ?? throw new ItemNotFoundException(typeof(Game), updateModel.OldGameKey,
                                                      nameof(updateModel.OldGameKey));
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
            await _mediator.Publish(new GameKeyUpdatedNotification(updateModel.OldGameKey, updateModel.Key));
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

    private async Task<IEnumerable<Guid>> GetGenresWithChildrenAsync(IEnumerable<Guid> genresIds)
    {
        var genres = await GenreRepository.GetBySpecAsync(new GenresByIdsWithDetailsSpec(genresIds));
        var allChildren = GetAllChildrenGenres(genres);

        var result = allChildren.Select(genre => genre.Id);

        return result;
    }

    private IEnumerable<Genre> GetAllChildrenGenres(IEnumerable<Genre> genres)
    {
        var result = genres.ToList();

        foreach (var genre in genres)
        {
            var children = GetAllChildrenGenres(genre.SubGenres);
            result.AddRange(children);
        }

        return result;
    }

    private async Task UpdateGameValues(Game game, GameUpdateModel updateModel)
    {
        await AssertGenresExistsAsync(updateModel.GenresIds);
        await AssertPlatformsExistsAsync(updateModel.PlatformsIds);

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

    private async Task AssertGenresExistsAsync(IEnumerable<Guid> genresIds)
    {
        foreach (var genreId in genresIds)
        {
            if (await GenreRepository.AnyAsync(new GenreByIdSpec(genreId)) == false)
            {
                throw new ItemNotFoundException(typeof(Genre), genreId);
            }
        }
    }

    private async Task AssertPlatformsExistsAsync(IEnumerable<Guid> platformsIds)
    {
        foreach (var platformId in platformsIds)
        {
            if (await PlatformTypesRepository.AnyAsync(new PlatformTypeByIdSpec(platformId)) == false)
            {
                throw new ItemNotFoundException(typeof(PlatformType), platformId);
            }
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