using GameStore.Core.Interfaces;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.SharedKernel.Interfaces.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Microsoft.Extensions.Logging;

namespace GameStore.Core.Services
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GameService> _logger;

        public GameService(IUnitOfWork unitOfWork, ILogger<GameService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        private IRepository<Game> GameRepository => _unitOfWork.GetRepository<Game>();
        private IRepository<Genre> GenreRepository => _unitOfWork.GetRepository<Genre>();

        public async Task<Game> CreateAsync(string key, string name, string description, byte[] file)
        {
            var game = new Game(key, name, description, file);

            var result = await GameRepository.AddAsync(game);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Game created { Key = {0}, Name = {1}}", key, name);

            return result;
        }    

        public async Task<ICollection<Game>> GetAllAsync()
        {
            return await GameRepository.GetBySpecificationAsync(new GamesWithDetails());
        }

        public async Task<ICollection<Game>> GetByGenreAsync(Genre genre)
        {
            return await GameRepository.GetBySpecificationAsync(new GameByGenreSpec(genre));
        }

        public async Task<Game> GetByKeyAsync(string key)
        {
            return (await GameRepository.GetBySpecificationAsync(new GameByKeySpec(key))).First();
        }

        public async Task<ICollection<Game>> GetByPlatformTypesAsync(params PlatformType[] platformTypes)
        {
            return await GameRepository.GetBySpecificationAsync(new GameByPlatformTypes(platformTypes));
        }

        public async Task<Game> ApplyGenreAsync(Guid gameId, Guid genreId)
        {
            var game = await GameRepository.GetByIdAsync(gameId);
            var genre = await GenreRepository.GetByIdAsync(genreId);

            if(game is null)
            {
                _logger.LogInformation("Add genre to game - no game with such id {0}", gameId);
                throw new ArgumentException("Game with such id doesnt exist");
            }

            if(genre is null)
            {
                _logger.LogInformation("Add genre to game - no genre with such id {0}", genreId);
                throw new ArgumentException("Genre with such id doesnt exist");
            }

            game.Genres.Add(genre);
            
            GameRepository.Update(game);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Game {0} added to genre {1}", game.Name, genre.Name);

            return game;
        }

        public async Task UpdateAsync(Game game)
        {
            GameRepository.Update(game);

            _logger.LogInformation("Game updated - {0}", game.Name);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var game = await GameRepository.GetByIdAsync(id);
            
            if(game is null)
            {
                _logger.LogInformation("Game delete failed - no game with such id {0}", id);
                return;
            }

            GameRepository.Delete(game); // TODO: Add overload to repository method

            _logger.LogInformation("Game deleted - {0}", game.Name);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<byte[]> GetFileAsync(string gameKey)
        {
            var game = (await GameRepository.GetBySpecificationAsync(new GameByKeySpec(gameKey))).FirstOrDefault();

            if(game is null)
            {
                _logger.LogInformation("Download failed - no game with such key {0}", gameKey);
                throw new InvalidOperationException();
            }

            _logger.LogInformation("File {0} downloaded", game.Name);

            return game.File;
        }
    }
}
