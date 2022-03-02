using GameStore.Core.Interfaces;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.SharedKernel.Interfaces.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Core.Services
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GameService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private IRepository<Game> GameRepository => _unitOfWork.GetRepository<Game>();
        private IRepository<Genre> GenreRepository => _unitOfWork.GetRepository<Genre>();

        public async Task<Game> CreateAsync(string key, string name, string description, byte[] file)
        {
            var game = new Game(key, name, description, file);

            var result = await GameRepository.AddAsync(game);
            await _unitOfWork.SaveChangesAsync();

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

            game.Genres.Add(genre);
            
            GameRepository.Update(game);

            await _unitOfWork.SaveChangesAsync();

            return game;
        }

        public async Task UpdateAsync(Game game)
        {
            GameRepository.Update(game);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var game = await GameRepository.GetByIdAsync(id);
            GameRepository.Delete(game); // TODO: Add overload to repository method
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<byte[]> GetFileAsync(string gameKey)
        {
            var game = (await GameRepository.GetBySpecificationAsync(new GameByKeySpec(gameKey))).First();

            return game.File;
        }
    }
}
