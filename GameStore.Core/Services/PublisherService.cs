using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Events.Notifications;
using GameStore.Core.Exceptions;
using GameStore.Core.Extensions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.Loggers;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Mongo.Suppliers;
using GameStore.Core.Models.Publishers;
using GameStore.Core.Models.Publishers.Specifications;
using GameStore.Core.Models.ServiceModels.Publishers;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MediatR;
using MongoDB.Bson;

namespace GameStore.Core.Services;

public class PublisherService : IPublisherService
{
    private readonly ISearchService _searchService;
    private readonly IMediator _mediator;
    private readonly IMongoLogger _mongoLogger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PublisherService(ISearchService searchService, IMediator mediator, IMongoLogger mongoLogger,
                            IUnitOfWork unitOfWork, IMapper mapper)
    {
        _searchService = searchService;
        _mediator = mediator;
        _mongoLogger = mongoLogger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private IRepository<Publisher> PublishersRepository => _unitOfWork.GetEfRepository<Publisher>();
    private IRepository<Supplier> SuppliersRepository => _unitOfWork.GetMongoRepository<Supplier>();

    public async Task<ICollection<PublisherDto>> GetAllAsync()
    {
        var filteredPublishers = await PublishersRepository.GetBySpecAsync(new PublishersListSpec());
        var filteredSuppliers = await SuppliersRepository.GetBySpecAsync();

        var mappedPublishers = _mapper.Map<IEnumerable<PublisherDto>>(filteredPublishers);
        var mappedSuppliers = _mapper.Map<IEnumerable<PublisherDto>>(filteredSuppliers);

        var result = mappedPublishers.Concat(mappedSuppliers);
        result = result.DistinctBy(dto => dto.Name);

        return result.ToList();
    }

    public async Task<PublisherDto> GetByCompanyNameAsync(string companyName)
    {
        var result = await _searchService.GetPublisherDtoByCompanyNameOrDefaultAsync(companyName)
                     ?? throw new ItemNotFoundException(typeof(Publisher), companyName);

        return result;
    }

    public async Task<Publisher> CreateAsync(PublisherCreateModel createModel)
    {
        var publisher = _mapper.Map<Publisher>(createModel);

        await PublishersRepository.AddAsync(publisher);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogCreateAsync(publisher);

        return publisher;
    }

    public async Task UpdateAsync(PublisherUpdateModel updateModel)
    {
        var publisher = await PublishersRepository.GetSingleOrDefaultBySpecAsync(new PublisherByNameSpec(updateModel.OldName))
                        ?? throw new ItemNotFoundException(typeof(Publisher), updateModel.OldName, nameof(updateModel.OldName));

        var oldPublisherVersion = publisher.ToBsonDocument();
        
        UpdatePublisherValues(publisher, updateModel);

        await PublishersRepository.UpdateAsync(publisher);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogUpdateAsync(typeof(Publisher), oldPublisherVersion, publisher.ToBsonDocument());

        if (updateModel.IsNameChanged)
        {
            await _mediator.Publish(new PublisherNameUpdatedNotification(updateModel.OldName, updateModel.Name));
        }
    }

    public async Task DeleteAsync(string companyName)
    {
        var publisher = await PublishersRepository.GetSingleOrDefaultBySpecAsync(new PublisherByNameSpec(companyName))
                        ?? throw new ItemNotFoundException(typeof(Publisher), companyName);

        publisher.IsDeleted = true;

        await PublishersRepository.UpdateAsync(publisher);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogDeleteAsync(publisher);
    }

    public async Task<bool> IsCompanyNameAlreadyExists(string companyName)
    {
        return await PublishersRepository.AnyAsync(new PublisherByNameSpec(companyName));
    }

    private void UpdatePublisherValues(Publisher publisher, PublisherUpdateModel updateModel)
    {
        publisher.Name = updateModel.Name;
        publisher.Description = updateModel.Description;
        publisher.HomePage = updateModel.HomePage;
    }
}