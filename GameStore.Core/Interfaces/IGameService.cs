using GameStore.Core.Models.Games;
using GameStore.Core.Models.Records;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.Core.Interfaces
{
    public interface IGameService
    {
        Task<ICollection<Game>> GetAllAsync();
        Task<ICollection<Game>> GetByGenreAsync(Genre genre);
        Task<ICollection<Game>> GetByPlatformTypesAsync(PlatformType[] platformTypes);
        Task<Game> GetByKeyAsync(string key);
        Task<Game> CreateAsync(GameCreateModel createModel);
        Task<Game> ApplyGenreAsync(Guid gameId, Guid genreId);
        Task UpdateAsync(GameUpdateModel updateModel);
        Task DeleteAsync(Guid id);
        Task<byte[]> GetFileAsync(string gameKey);
    }
}
