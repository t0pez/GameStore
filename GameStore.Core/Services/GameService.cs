using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
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

        public GameService(IUnitOfWork unitOfWork, ILogger<GameService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        private IRepository<Game> GameRepository => _unitOfWork.GetRepository<Game>();
        private IRepository<Genre> GenreRepository => _unitOfWork.GetRepository<Genre>();

        public async Task<Game> CreateAsync(string key, string name, string description, byte[] file, CancellationToken token = default)
        {
            try
            {
                var game = new Game(key, name, description, file);

                var result = await GameRepository.AddAsync(game, token);
                await _unitOfWork.SaveChangesAsync(token);

                _logger.LogInformation("Game created [ Key = {0}, Name = {1}]", key, name);

                return result;
            }
            catch (ItemAlreadyExistsException)
            {
                _logger.LogInformation("Game creation failed - game with such id already exists");
                return await CreateAsync(key, name, description, file, token); // Re-generate Id and repeat operation
            }
        }    

        public async Task<ICollection<Game>> GetAllAsync(CancellationToken token = default)
        {
            return await GameRepository.GetBySpecAsync(new GamesWithDetails(), token);
        }

        public async Task<ICollection<Game>> GetByGenreAsync(Genre genre, CancellationToken token = default)
        {
            return await GameRepository.GetBySpecAsync(new GamesByGenreSpec(genre), token);
        }

        public async Task<Game> GetByKeyAsync(string key, CancellationToken token = default)
        {
            try
            {
                var result = await GameRepository.GetSingleBySpecAsync(new GameByKeySpec(key), token);

                return result;
            }
            catch (ItemNotFoundException<Game>)
            {
                throw new ArgumentException("Game with such key doesnt exist");
            }
        }

        public async Task<ICollection<Game>> GetByPlatformTypesAsync(PlatformType[] platformTypes, CancellationToken token = default)
        {
            return await GameRepository.GetBySpecAsync(new GamesByPlatformTypes(platformTypes), token);
        }

        public async Task<Game> ApplyGenreAsync(Guid gameId, Guid genreId, CancellationToken token = default)
        {
            try
            {
                var game = await GameRepository.GetByIdAsync(gameId, token);
                var genre = await GenreRepository.GetByIdAsync(genreId, token);

                game.Genres.Add(genre);

                await GameRepository.UpdateAsync(game, token);

                await _unitOfWork.SaveChangesAsync(token);

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

        public async Task UpdateAsync(Game game, CancellationToken token = default)
        {
            try
            {
                await GameRepository.UpdateAsync(game, token);

                _logger.LogInformation("Game updated - {0}", game.Name);

                await _unitOfWork.SaveChangesAsync(token);
            }
            catch (ItemNotFoundException<Game>)
            {
                _logger.LogInformation("Update game failed - no game with such id {0}", game.Id);
                throw new InvalidOperationException();
            }
        }

        public async Task DeleteAsync(Guid id, CancellationToken token = default)
        {
            try
            {
                var game = await GameRepository.GetByIdAsync(id, token);

                await GameRepository.DeleteAsync(game, token); // TODO: Add overload to repository method

                await _unitOfWork.SaveChangesAsync(token);
                
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

        public async Task<byte[]> GetFileAsync(string gameKey, CancellationToken token = default)
        {
            try
            {
                var game = await GameRepository.GetSingleBySpecAsync(new GameByKeySpec(gameKey), token);

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
