using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Server.Publishers;
using GameStore.Core.Models.ServiceModels.Publishers;

namespace GameStore.Core.Interfaces;

public interface IPublisherService
{
    public Task<ICollection<PublisherDto>> GetAllAsync();
    public Task<PublisherDto> GetByCompanyNameAsync(string companyName);
    public Task<Publisher> CreateAsync(PublisherCreateModel createModel);
    public Task UpdateAsync(PublisherUpdateModel updateModel);
    public Task DeleteAsync(string companyName);
    public Task<bool> IsCompanyNameAlreadyExists(string companyName);
}