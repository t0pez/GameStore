using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Publishers;
using GameStore.Core.Models.Publishers.Specifications;
using GameStore.Core.Models.ServiceModels.Publishers;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.Extensions.Logging;

namespace GameStore.Core.Services;

public class PublisherService : IPublisherService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PublisherService> _logger;

    public PublisherService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PublisherService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    private IRepository<Publisher> Repository => _unitOfWork.GetRepository<Publisher>();

    public async Task<ICollection<Publisher>> GetAllAsync()
    {
        var result = await Repository.GetBySpecAsync(new PublishersWithDetailsSpec());

        return result;
    }

    public async Task<Publisher> GetByCompanyNameAsync(string companyName)
    {
        var result = await Repository.GetSingleOrDefaultBySpecAsync(new PublisherByCompanyName(companyName))
                     ?? throw new ItemNotFoundException(typeof(Publisher), companyName);

        return result;
    }

    public async Task<Publisher> CreateAsync(PublisherCreateModel createModel)
    {
        var publisher = _mapper.Map<Publisher>(createModel); // TODO: add map

        await Repository.AddAsync(publisher);
        await _unitOfWork.SaveChangesAsync();

        return publisher;
    }

    public async Task UpdateAsync(PublisherUpdateModel updateModel)
    {
        var publisher = await Repository.GetSingleOrDefaultBySpecAsync(new PublisherByIdWithDetailsSpec(updateModel.Id))
                        ?? throw new ItemNotFoundException(typeof(Publisher), updateModel.Id, nameof(updateModel.Id));

        await UpdatePublisherValues(publisher, updateModel);

        await Repository.UpdateAsync(publisher);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var publisher = await Repository.GetSingleOrDefaultBySpecAsync(new PublisherByIdSpec(id))
                        ?? throw new ItemNotFoundException(typeof(Publisher), id);

        publisher.IsDeleted = true;

        await Repository.UpdateAsync(publisher);
        await _unitOfWork.SaveChangesAsync();
    }
    
    private async Task UpdatePublisherValues(Publisher publisher, PublisherUpdateModel updateModel)
    {
        publisher.Name = updateModel.Name;
        publisher.Description = updateModel.Description;
        publisher.HomePage = updateModel.HomePage;
        // TODO: update relationships
    }
}