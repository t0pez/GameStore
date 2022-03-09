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
            _gameKeyAliasCraft =
                new AliasCraftBuilder()
                .AddPairToReplace('_', '-')
                .AddSymbolsToRemove(' ', '\'', ',', '.', ':')
                .Build();
        }

        private IRepository<Game> GameRepository => _unitOfWork.GetRepository<Game>();
        private IRepository<Genre> GenreRepository => _unitOfWork.GetRepository<Genre>();
        private IRepository<PlatformType> PlatformTypesRepository => _unitOfWork.GetRepository<PlatformType>();

        public async Task<Game> CreateAsync(GameCreateModel model)
        {
            var gameKey = _gameKeyAliasCraft.CreateAlias(model.Name);

            if (await GameRepository.AnyAsync(new GameByKeySpec(gameKey)))
            {
                throw new InvalidOperationException("Game with this key already exists. " +
                    $"GameKey = {gameKey}");
            }

            var game = new Game(gameKey, model.Name, model.Description, model.File);

            await GameRepository.AddAsync(game);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Game created [ Key = {game.Key}, Name = {game.Name} ]");

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
                throw new ArgumentException($"Game with such id doesnt exist. Id = {gameId}");
            }

            var genre = await GenreRepository.GetByIdAsync(genreId);

            if (genre is null)
            {
                throw new ArgumentException($"Genre with such id doesnt exist. Id = {genreId}");
            }

            game.Genres.Add(genre);

            GameRepository.Update(game);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Game added to genre. " +
                $"Game.Name = {game.Name}, Genre.Name = {genre.Name}");

            return game;
        }

        public async Task UpdateAsync(GameUpdateModel updateModel)
        {
            var game = await GameRepository.GetByIdAsync(updateModel.Id);

            if (game is null)
            {
                throw new InvalidOperationException("Game with such id doesnt exists." +
                    $"Id = {updateModel.Id}");
            }

            SetUpdatedValues(game, updateModel);

            GameRepository.Update(game);

            _logger.LogInformation($"Game updated. Game.Name = {updateModel.Name}");

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var game = await GameRepository.GetByIdAsync(id);

            if (game is null)
            {
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
                throw new InvalidOperationException("Game with such key doesnt exists." +
                    $"GameKey = {gameKey}");
            }

            _logger.LogInformation($"File downloaded. Game.Name = {game.Name}");

            return game.File;
        }

        private void SetUpdatedValues(Game game, GameUpdateModel updateModel)
        {
            CheckIfGenresExists(updateModel);
            CheckIfPlatformTypesExists(updateModel);

            game.Name = updateModel.Name;
            game.Description = updateModel.Description;
            game.File = updateModel.File;
            game.Genres = updateModel.Genres;
            game.PlatformTypes = updateModel.PlatformTypes;
            // TODO: ask PO about re-generation key
        }

        private void CheckIfGenresExists(GameUpdateModel updateModel)
        {
            foreach (var genre in updateModel.Genres)
            {
                // ExceptionMiddleware doesnt work with await here
                if (GenreRepository.AnyAsync(genre.Id).Result == false)
                {
                    throw new ArgumentException($"Genre doesnt exists. Id = {genre.Id}");
                }
            }
        }

        private void CheckIfPlatformTypesExists(GameUpdateModel updateModel)
        {
            foreach (var platformType in updateModel.PlatformTypes)
            {
                if (PlatformTypesRepository.AnyAsync(platformType.Id).Result == false)
                {
                    throw new ArgumentException($"Platform type doesnt exists. Id = {platformType.Id}");
                }
            }
        }
    }
}
