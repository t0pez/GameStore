using GameStore.Core.Models.Games;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Core.Interfaces
{
    public interface IGameService
    {
        Task<ICollection<Game>> GetAllAsync(CancellationToken token = default);
        Task<ICollection<Game>> GetByGenreAsync(Genre genre, CancellationToken token = default);
        Task<ICollection<Game>> GetByPlatformTypesAsync(PlatformType[] platformTypes, CancellationToken token = default);
        Task<Game> GetByKeyAsync(string key, CancellationToken token = default);
        Task<Game> CreateAsync(string key, string name, string description, byte[] file, CancellationToken token = default);
        Task<Game> ApplyGenreAsync(Guid gameId, Guid genreId, CancellationToken token = default);
        Task UpdateAsync(Game game, CancellationToken token = default);
        Task DeleteAsync(Guid id, CancellationToken token = default);
        Task<byte[]> GetFileAsync(string gameKey, CancellationToken token = default);
    }
}
