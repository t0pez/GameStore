using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces.RelationshipModelsServices;
using GameStore.Core.Models.Games.Specifications.Filters;
using GameStore.Core.Models.Genres;
using GameStore.Core.Models.Genres.Specifications;
using GameStore.Core.Models.PlatformTypes;
using GameStore.Core.Models.PlatformTypes.Specifications;
using GameStore.Core.Models.RelationalModels;
using GameStore.Core.Models.RelationalModels.Specifications;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.Core.PagedResult;

namespace GameStore.Core.Services;

public class GameService : IGameService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GameService> _logger;
    private readonly IMapper _mapper;
    private readonly IRelationshipModelService<GameGenre> _gameGenreService;
    private readonly IRelationshipModelService<GamePlatformType> _gamePlatformService;

    public GameService(IUnitOfWork unitOfWork, ILogger<GameService> logger, IMapper mapper,
                       IRelationshipModelService<GameGenre> gameGenreService,
                       IRelationshipModelService<GamePlatformType> gamePlatformService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _gameGenreService = gameGenreService;
        _gamePlatformService = gamePlatformService;
       
    }

    private IRepository<Game> GameRepository => _unitOfWork.GetRepository<Game>();
    private IRepository<Genre> GenreRepository => _unitOfWork.GetRepository<Genre>();
    private IRepository<PlatformType> PlatformTypesRepository => _unitOfWork.GetRepository<PlatformType>();

    public async Task<Game> CreateAsync(GameCreateModel model)
    {
        var game = _mapper.Map<Game>(model);
        game.AddedToStoreAt = DateTime.UtcNow;

        await GameRepository.AddAsync(game);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Game created. " +
                               $"{nameof(game.Id)} = {game.Id}");

        return game;
    }

    public Task<int> GetTotalCountAsync()
    {
        return GameRepository.CountAsync(new GamesListSpec());
    }

    public async Task<List<Game>> GetAllAsync()
    {
        var result = await GameRepository.GetBySpecAsync(new GamesListSpec());

        return result;
    }

    public async Task<PagedResult<Game>> GetByFilterAsync(GameSearchFilter filter)
    {
        if (filter.GenresIds.Any())
        {
            var genresWithChildren = await GetGenresWithChildrenAsync(filter.GenresIds);
            filter.GenresIds = genresWithChildren;
        }

        var games = await GameRepository.GetBySpecAsync(new GamesByFilterSpec(filter).EnablePaging(filter));
        var totalGamesCount = await GameRepository.CountAsync(new GamesByFilterSpec(filter));

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
        var game = await GameRepository.GetSingleOrDefaultBySpecAsync(new GameByIdWithDetailsSpec(updateModel.Id))
                   ?? throw new ItemNotFoundException(typeof(Game), updateModel.Id, nameof(updateModel.Id));
        
        await UpdateGameValues(game, updateModel);

        await GameRepository.UpdateAsync(game);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Game updated. " +
                               $"{nameof(game.Id)} = {game.Id}");
    }

    public async Task DeleteAsync(Guid id)
    {
        var game = await GameRepository.GetSingleOrDefaultBySpecAsync(new GameByIdSpec(id))
                   ?? throw new ItemNotFoundException(typeof(Game), id);
        
        game.IsDeleted = true;
        await GameRepository.UpdateAsync(game);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Game marked as deleted. " +
                               $"{nameof(game.Id)} = {game.Id}");
    }

    public async Task<byte[]> GetFileAsync(string gameKey)
    {
        var game = await GameRepository.GetSingleOrDefaultBySpecAsync(new GameByKeySpec(gameKey))
                   ?? throw new ItemNotFoundException(typeof(Game), gameKey);

        return game.File;
    }

    private async Task<IEnumerable<Guid>> GetGenresWithChildrenAsync(IEnumerable<Guid> genresIds)
    {
        var genres = await GenreRepository.GetBySpecAsync(new GenresByIdsWithDetails(genresIds));
        var allChildren= GetAllChildrenGenres(genres);

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
        game.PublisherId = updateModel.PublisherId;
    }

    private async Task AssertGenresExistsAsync(IEnumerable<Guid> genresIds)
    {
        foreach (var genreId in genresIds)
            if (await GenreRepository.AnyAsync(new GenreByIdSpec(genreId)) == false)
                throw new ItemNotFoundException(typeof(Genre), genreId);
    }

    private async Task AssertPlatformsExistsAsync(IEnumerable<Guid> platformsIds)
    {
        foreach (var platformId in platformsIds)
            if (await PlatformTypesRepository.AnyAsync(new PlatformTypeByIdSpec(platformId)) == false)
                throw new ItemNotFoundException(typeof(PlatformType), platformId);
    }

    private async Task UpdateGameRelationshipModelsAsync(GameUpdateModel updateModel)
    {
        var newGameGenres =
            updateModel.GenresIds.Select(id => new GameGenre { GameId = updateModel.Id, GenreId = id });
        var newGamePlatforms =
            updateModel.PlatformsIds.Select(id => new GamePlatformType { GameId = updateModel.Id, PlatformId = id });

        await _gameGenreService.UpdateManyToManyAsync(newGameGenres, new GameGenresByGameIdSpec(updateModel.Id));
        await _gamePlatformService.UpdateManyToManyAsync(newGamePlatforms,
                                                         new GamePlatformsByPlatformIdSpec(updateModel.Id));
    }
}