using System;
using System.Threading.Tasks;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.ServiceModels.Enums;
using GameStore.Core.Models.ServiceModels.Games;

namespace GameStore.Core.Interfaces;

public interface IGameService
{
    public Task<int> GetTotalCountAsync();
    public Task<Game> GetByKeyAsync(string key);
    public Task<Game> GetByIdAsync(Guid id);
    public Task<Game> CreateAsync(GameCreateModel createModel);
    public Task UpdateAsync(GameUpdateModel updateModel);
    public Task UpdateFromEndpointAsync(GameUpdateModel updateModel);
    public Task DeleteAsync(string gameKey, Database database);
    public Task<byte[]> GetFileAsync(string gameKey);
    public Task<bool> IsGameKeyAlreadyExists(string gameKey);
}