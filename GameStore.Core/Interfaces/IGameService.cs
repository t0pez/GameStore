using GameStore.Core.Models.Games;
using System;
using System.Threading.Tasks;
using GameStore.Core.Models.Games.Specifications.Filters;
using GameStore.Core.Models.ServiceModels.Enums;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.Core.PagedResult;

namespace GameStore.Core.Interfaces;

public interface IGameService
{
    public Task<int> GetTotalCountAsync();
    public Task<PagedResult<Game>> GetByFilterAsync(GameSearchFilter filter);
    public Task<Game> GetByKeyAsync(string key);
    public Task<Game> GetByIdAsync(Guid id);
    public Task<Game> CreateAsync(GameCreateModel createModel);
    public Task UpdateAsync(GameUpdateModel updateModel);
    public Task UpdateFromEndpointAsync(GameUpdateModel updateModel);
    public Task DeleteAsync(string gameKey, Database database);
    public Task<byte[]> GetFileAsync(string gameKey);
    public Task<bool> IsGameKeyAlreadyExists(string gameKey);
}