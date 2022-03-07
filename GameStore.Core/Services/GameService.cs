﻿using GameStore.Core.Exceptions;
using GameStore.Core.Helpers.AliasCrafting;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.Core.Models.Records;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Core.Services
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GameService> _logger;
        private readonly IAliasCraft _gameKeyAliasCraft;

        public GameService(IUnitOfWork unitOfWork, ILogger<GameService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _gameKeyAliasCraft = new AliasCraft(
                replacingPairs: new(),
                symbolsToDelete: new List<char> {
                    ' ', '\'', ',', '.', ':',
                }
            );
        }

        private IRepository<Game> GameRepository => _unitOfWork.GetRepository<Game>();
        private IRepository<Genre> GenreRepository => _unitOfWork.GetRepository<Genre>();

        public async Task<Game> CreateAsync(CreateGameModel model)
        {
            var gameKey = _gameKeyAliasCraft.CreateAlias(model.Name);
            var game = new Game(gameKey, model.Name, model.Description, model.File);

            await GameRepository.AddAsync(game);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Game created [ Key = {0}, Name = {1}]", game.Key, model.Name);

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

        public async Task<Game> GetByKeyAsync(string key)
        {
            var result = await GameRepository.GetSingleBySpecAsync(new GameByKeySpec(key));

            if (result is null)
            {
                throw new ItemNotFoundException($"Game not found. GameKey = {key}");
            }

            return result;
        }

        public async Task<ICollection<Game>> GetByPlatformTypesAsync(PlatformType[] platformTypes)
        {
            var result = await GameRepository.GetBySpecAsync(new GamesByPlatformTypesSpec(platformTypes));

            return result;
        }

        public async Task<Game> ApplyGenreAsync(Guid gameId, Guid genreId)
        {
            var game = await GameRepository.GetByIdAsync(gameId);

            if (game is null)
            {
                _logger.LogInformation("Add genre to game failed");
                throw new ArgumentException($"Game with such id doesnt exist. Id = {gameId}");
            }

            var genre = await GenreRepository.GetByIdAsync(genreId);

            if (genre is null)
            {
                _logger.LogInformation("Add genre to game failed");
                throw new ArgumentException($"Genre with such id doesnt exist. Id = {genreId}");
            }

            game.Genres.Add(genre);

            GameRepository.Update(game);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Game added to genre. " +
                $"Game.Name = {game.Name}, Genre.Name = {genre.Name}");

            return game;
        }

        public async Task UpdateAsync(Game game)
        {
            GameRepository.Update(game);

            _logger.LogInformation($"Game updated. Game.Name = {game.Name}");

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var game = await GameRepository.GetByIdAsync(id);

            if (game is null)
            {
                _logger.LogInformation("Game delete failed");
                throw new InvalidOperationException($"Game with such id doesnt exists." +
                    $"Id = {id}");
            }

            GameRepository.Delete(game);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Game deleted. Game.Name = {game.Name}");
        }

        public async Task<byte[]> GetFileAsync(string gameKey)
        {
            var game = await GameRepository.GetSingleBySpecAsync(new GameByKeySpec(gameKey));

            if (game is null)
            {
                _logger.LogInformation("Download failed");
                throw new InvalidOperationException("Game with such key doesnt exists." +
                    $"GameKey = {gameKey}");
            }

            _logger.LogInformation($"File downloaded. Game.Name = {game.Name}");

            return game.File;
        }
    }
}
