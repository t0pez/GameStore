using GameStore.Core.Models.Games;
using GameStore.Core.Models.Records;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.Core.Interfaces;

public interface IGameService
{
    public Task<int> GetTotalCountAsync();
    public Task<ICollection<Game>> GetAllAsync();
    public Task<ICollection<Game>> GetByGenreAsync(Guid genreId);
    public Task<ICollection<Game>> GetByPlatformTypesAsync(ICollection<Guid> platformTypesIds);
    public Task<Game> GetByKeyAsync(string key);
    public Task<Game> CreateAsync(GameCreateModel createModel);
    public Task UpdateAsync(GameUpdateModel updateModel);
    public Task DeleteAsync(Guid id);
    public Task<byte[]> GetFileAsync(string gameKey);
}