using GameStore.Core.Models.Games;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.Core.Interfaces
{
    public interface IGameService
    {
        Task<ICollection<Game>> GetAllAsync();
        Task<ICollection<Game>> GetByGenreAsync(Genre genre);
        Task<ICollection<Game>> GetByPlatformTypesAsync(params PlatformType[] platformTypes);
        Task<Game> GetByKeyAsync(string key);
        Task<Game> CreateAsync(string key, string name, string description, byte[] file);
        Task<Game> ApplyGenreAsync(Guid gameId, Guid genreId);
        Task UpdateAsync(Game game);
        Task DeleteAsync(Guid id);
        Task<byte[]> GetFileAsync(string gameKey); // TODO: ????
    }
}
