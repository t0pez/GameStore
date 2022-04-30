using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Core.Models.PlatformTypes;
using GameStore.Core.Models.ServiceModels.PlatformTypes;

namespace GameStore.Core.Interfaces;

public interface IPlatformTypeService
{
    public Task<ICollection<PlatformType>> GetAllAsync();
    public Task<PlatformType> GetByIdAsync(Guid id);
    public Task CreateAsync(PlatformTypeCreateModel createModel);
    public Task UpdateAsync(PlatformTypeUpdateModel updateModel);
    public Task DeleteAsync(Guid id);
}