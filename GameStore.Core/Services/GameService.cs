using GameStore.Core.Exceptions;
using GameStore.Core.Helpers.AliasCrafting;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.Core.Models.Records;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces.RelationshipModelsServices;
using GameStore.Core.Models.RelationalModels;
using GameStore.Core.Models.RelationalModels.Specifications;

namespace GameStore.Core.Services;

public class GameService : IGameService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GameService> _logger;
    private readonly IAliasCraft _gameKeyAliasCraft;
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
        _gameKeyAliasCraft =
            new AliasCraftBuilder()
                .Values("_", " ").ReplaceWith("-")
                .Values(",", ".", ":", "?").Delete()
                .Build();
    }

    private IRepository<Game> GameRepository => _unitOfWork.GetRepository<Game>();
    private IRepository<Genre> GenreRepository => _unitOfWork.GetRepository<Genre>();
    private IRepository<PlatformType> PlatformTypesRepository => _unitOfWork.GetRepository<PlatformType>();

    public async Task<Game> CreateAsync(GameCreateModel model)
    {
        var gameKey = await CreateUniqueGameKeyAsync(model.Name);

        var game = _mapper.Map<Game>(model);
        game.Key = gameKey;
        
        await GameRepository.AddAsync(game);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Game created. " +
                               $"{nameof(game.Id)} = {game.Id}");

        return game;
    }

    public async Task<ICollection<Game>> GetAllAsync()
    {
        var result = await GameRepository.GetBySpecAsync(new GamesWithDetailsSpec());

        return result;
    }

    public async Task<ICollection<Game>> GetByGenreAsync(Guid genreId)
    {
        var result = await GameRepository.GetBySpecAsync(new GamesByGenreSpec(genreId));

        return result;
    }

    public async Task<Game> GetByKeyAsync(string gameKey)
    {
        var result = await GameRepository.GetSingleBySpecAsync(new GameByKeyWithDetailsSpec(gameKey));

        if (result is null)
        {
            throw new ItemNotFoundException($"Game not found. {nameof(gameKey)} = {gameKey}");
        }

        return result;
    }

    public async Task<ICollection<Game>> GetByPlatformTypesAsync(ICollection<Guid> platformTypesIds)
    {
        var result = await GameRepository.GetBySpecAsync(new GamesByPlatformTypesSpec(platformTypesIds));

        return result;
    }

    public async Task UpdateAsync(GameUpdateModel updateModel)
    {
        var game = await GameRepository.GetSingleBySpecAsync(new GameByIdWithDetailsSpec(updateModel.Id));

        if (game is null)
        {
            throw new ItemNotFoundException("Game with such id doesnt exists." +
                                            $"Id = {updateModel.Id}");
        }

        await UpdateGameValues(game, updateModel);

        await GameRepository.UpdateAsync(game);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Game updated. " +
                               $"{nameof(game.Id)} = {game.Id}");
    }

    public async Task DeleteAsync(Guid id)
    {
        var game = await GameRepository.GetSingleBySpecAsync(new GameByIdSpec(id));

        if (game is null)
        {
            throw new ItemNotFoundException("Game with such id doesnt exists." +
                                            $"{nameof(game.Id)} = {id}");
        }

        game.IsDeleted = true;
        await GameRepository.UpdateAsync(game);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Game deleted. {nameof(game.Id)} = {game.Id}");
    }

    public async Task<byte[]> GetFileAsync(string gameKey)
    {
        var game = await GameRepository.GetSingleBySpecAsync(new GameByKeySpec(gameKey));

        if (game is null)
        {
            throw new ItemNotFoundException("Game with such key doesnt exists." +
                                            $"{nameof(game.Key)} = {gameKey}");
        }

        return game.File;
    }

    private async Task UpdateGameValues(Game game, GameUpdateModel updateModel)
    {
        await AssertGenresExistsAsync(updateModel.GenresIds);
        await AssertPlatformsExistsAsync(updateModel.PlatformsIds);
        
        await UpdateGameRelationshipModelsAsync(updateModel);

        game.Name = updateModel.Name;
        game.Description = updateModel.Description;
        game.File = updateModel.File;
    }

    private async Task AssertGenresExistsAsync(IEnumerable<Guid> genresIds)
    {
        foreach (var genreId in genresIds)
            if (await GenreRepository.AnyAsync(new GenreByIdSpec(genreId)) == false)
                throw new ItemNotFoundException($"Genre not found. {nameof(genreId)} = {genreId}");
    }
    
    private async Task AssertPlatformsExistsAsync(IEnumerable<Guid> platformsIds)
    {
        foreach (var platformId in platformsIds)
            if (await PlatformTypesRepository.AnyAsync(new PlatformTypeByIdSpec(platformId)) == false)
                throw new ItemNotFoundException($"Platform not found. {nameof(platformId)} = {platformId}");
    }

    private async Task UpdateGameRelationshipModelsAsync(GameUpdateModel updateModel)
    {
        await DeletePreviousGameRelationshipsAsync(updateModel.Id);
        await AddNewGameRelationshipsAsync(updateModel);
    }

    private async Task AddNewGameRelationshipsAsync(GameUpdateModel updateModel)
    {
        var newGameGenres =
            updateModel.GenresIds.Select(id => new GameGenre { GameId = updateModel.Id, GenreId = id });
        var newGamePlatforms =
            updateModel.PlatformsIds.Select(id => new GamePlatformType { GameId = updateModel.Id, PlatformId = id });

        await _gameGenreService.AddRangeAsync(newGameGenres);
        await _gamePlatformService.AddRangeAsync(newGamePlatforms);
    }

    private async Task DeletePreviousGameRelationshipsAsync(Guid gameId)
    {
        await _gameGenreService.DeleteBySpecAsync(new GameGenresByGameId(gameId));
        await _gamePlatformService.DeleteBySpecAsync(new GamePlatformsByPlatformId(gameId));
    }

    private async Task<string> CreateUniqueGameKeyAsync(string source)
    {
        _logger.LogDebug("Started creation game key for game with Name = {Name}", source);
        var gameKey = _gameKeyAliasCraft.CreateAlias(source);

        if (await IsKeyUniqueAsync(gameKey) == false)
        {
            return await CreateGameKeyWithCodeAsync(source);
        }

        return gameKey;
    }

    private async Task<string> CreateGameKeyWithCodeAsync(string source)
    {
        var attemptCode = 1;

        do
        {
            var gameKey = CreateAliasWithCode(source, attemptCode++);

            if (await IsKeyUniqueAsync(gameKey))
            {
                _logger.LogDebug("Ended creation game key for game with Name = {Name}, Result = {Key}", 
                                 source, gameKey);
                return gameKey;
            }
        } while (true);
    }

    private string CreateAliasWithCode(string source, int code)
    {
        return _gameKeyAliasCraft.CreateAlias(source + "--" + code);
    }

    private async Task<bool> IsKeyUniqueAsync(string gameKey)
    {
        return await GameRepository.AnyAsync(new GameByKeySpec(gameKey)) == false;
    }
}