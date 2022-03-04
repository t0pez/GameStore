using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
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

        public GameService(IUnitOfWork unitOfWork, ILogger<GameService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        private IRepository<Game> GameRepository => _unitOfWork.GetRepository<Game>();
        private IRepository<Genre> GenreRepository => _unitOfWork.GetRepository<Genre>();

        public async Task<Game> CreateAsync(string key, string name, string description, byte[] file)
        {
            try
            {
                var game = new Game(key, name, description, file);

                var result = await GameRepository.AddAsync(game);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Game created [ Key = {0}, Name = {1}]", key, name);

                return result;
            }
            catch (ItemAlreadyExistsException)
            {
                _logger.LogInformation("Game creation failed - game with such id already exists");
                return await CreateAsync(key, name, description, file); // Re-generate Id and repeat operation
            }
        }    

        public async Task<ICollection<Game>> GetAllAsync()
        {
            return await GameRepository.GetBySpecAsync(new GamesWithDetails());
        }

        public async Task<ICollection<Game>> GetByGenreAsync(Genre genre)
        {
            return await GameRepository.GetBySpecAsync(new GamesByGenreSpec(genre));
        }

        public async Task<Game> GetByKeyAsync(string key)
        {
            try
            {
                var result = await GameRepository.GetSingleBySpecAsync(new GameByKeySpec(key));

                return result;
            }
            catch (ItemNotFoundException<Game>)
            {
                throw new ArgumentException("Game with such key doesnt exist");
            }
        }

        public async Task<ICollection<Game>> GetByPlatformTypesAsync(params PlatformType[] platformTypes)
        {
            return await GameRepository.GetBySpecAsync(new GamesByPlatformTypes(platformTypes));
        }

        public async Task<Game> ApplyGenreAsync(Guid gameId, Guid genreId)
        {
            try
            {
                var game = await GameRepository.GetByIdAsync(gameId);
                var genre = await GenreRepository.GetByIdAsync(genreId);

                game.Genres.Add(genre);

                await GameRepository.UpdateAsync(game);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Game {0} added to genre {1}", game.Name, genre.Name);

                return game;
            }
            catch (ItemNotFoundException<Game>)
            {
                _logger.LogInformation("Add genre to game - no game with such id {0}", gameId);
                throw new ArgumentException("Game with such id doesnt exist");
            }
            catch(ItemNotFoundException<Genre>)
            {
                _logger.LogInformation("Add genre to game - no genre with such id {0}", genreId);
                throw new ArgumentException("Genre with such id doesnt exist");
            }
        }

        public async Task UpdateAsync(Game game)
        {
            try
            {
                await GameRepository.UpdateAsync(game);

                _logger.LogInformation("Game updated - {0}", game.Name);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (ItemNotFoundException<Game>)
            {
                _logger.LogInformation("Update game failed - no game with such id {0}", game.Id);
                throw new InvalidOperationException();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var game = await GameRepository.GetByIdAsync(id);

                await GameRepository.DeleteAsync(game); // TODO: Add overload to repository method

                await _unitOfWork.SaveChangesAsync();
                
                _logger.LogInformation("Game deleted - {0}", game.Name);
            }
            catch (ItemNotFoundException<Game>)
            {
                _logger.LogInformation("Game delete failed - no game with such id {0}", id);
                throw new InvalidOperationException();
            }
            catch (InvalidOperationException)
            {
                _logger.LogInformation("Game delete failed - game already marked as deleted {0}", id);
                // ignored
            }
        }

        public async Task<byte[]> GetFileAsync(string gameKey)
        {
            try
            {
                var game = await GameRepository.GetSingleBySpecAsync(new GameByKeySpec(gameKey));

                _logger.LogInformation("File {0} downloaded", game.Name);

                return game.File;
            }
            catch (ItemNotFoundException<Game>)
            {
                _logger.LogInformation("Download failed - no game with such key {0}", gameKey);
                throw new InvalidOperationException();
            }
        }
    }
}
