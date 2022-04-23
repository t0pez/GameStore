using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Core.Models.Publishers;
using GameStore.Core.Models.ServiceModels.Publishers;

namespace GameStore.Core.Interfaces;

public interface IPublisherService
{
    public Task<ICollection<Publisher>> GetAll();
    public Task<Publisher> GetByCompanyName(string companyName);
    public Task<Publisher> CreateAsync(PublisherCreateModel createModel);
    public Task UpdateAsync(PublisherUpdateModel updateModel);
    public Task DeleteAsync(Guid id);
}