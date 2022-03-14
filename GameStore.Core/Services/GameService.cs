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
using System.Threading.Tasks;
using AutoMapper;

namespace GameStore.Core.Services;

public class GameService : IGameService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GameService> _logger;
    private readonly IAliasCraft _gameKeyAliasCraft;
    private readonly IMapper _mapper;

    public GameService(IUnitOfWork unitOfWork, ILogger<GameService> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _gameKeyAliasCraft =
            new AliasCraftBuilder()
                .AddPairToReplace("_", "-")
                .AddPairToReplace(" ", "-")
                .AddSymbolsToRemove(",", ".", ":", "?")
                .Build();
    }

    private IRepository<Game> GameRepository => _unitOfWork.GetRepository<Game>();
    private IRepository<Genre> GenreRepository => _unitOfWork.GetRepository<Genre>();
    private IRepository<PlatformType> PlatformTypesRepository => _unitOfWork.GetRepository<PlatformType>();

    public async Task<Game> CreateAsync(GameCreateModel model)
    {
        var gameKey = await CreateUniqueGameKey(model.Name);

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

    public async Task<ICollection<Game>> GetByGenreAsync(Genre genre)
    {
        var result = await GameRepository.GetBySpecAsync(new GamesByGenreSpec(genre));

        return result;
    }

    public async Task<Game> GetByKeyAsync(string gameKey)
    {
        var result = await GameRepository.GetSingleBySpecAsync(new GameByKeySpec(gameKey));

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

        await SetUpdatedValues(game, updateModel);

        await GameRepository.UpdateAsync(game);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Game updated. " +
                               $"{nameof(game.Id)} = {game.Id}");
    }

    public async Task DeleteAsync(Guid id)
    {
        var game = await GameRepository.GetByIdAsync(id);

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

    private async Task SetUpdatedValues(Game game, GameUpdateModel updateModel)
    {
        var genres = await GenreRepository.GetBySpecAsync(new GenresByIdsSpec(updateModel.GenresIds));
        var platformTypes = await
            PlatformTypesRepository.GetBySpecAsync(new PlatformTypesByIdsSpec(updateModel.PlatformTypesIds));

        game.Name = updateModel.Name;
        game.Description = updateModel.Description;
        game.File = updateModel.File;
        game.Genres = genres;
        game.PlatformTypes = platformTypes;
    }

    private async Task<string> CreateUniqueGameKey(string source)
    {
        var gameKey = _gameKeyAliasCraft.CreateAlias(source);

        if (await KeyIsUnique(gameKey) == false)
        {
            return await CreateGameKeyWithCode(source);
        }

        return gameKey;
    }

    private async Task<string> CreateGameKeyWithCode(string source)
    {
        var attemptCode = 1;

        do
        {
            var gameKey = CreateAliasWithCode(source, attemptCode++);

            if (await KeyIsUnique(gameKey))
            {
                return gameKey;
            }
        } while (true);
    }

    private string CreateAliasWithCode(string source, int code)
    {
        return _gameKeyAliasCraft.CreateAlias(source + "--" + code);
    }

    private async Task<bool> KeyIsUnique(string gameKey)
    {
        return await GameRepository.AnyAsync(new GameByKeySpec(gameKey)) == false;
    }
}