using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.Loggers;
using GameStore.Core.Models.PlatformTypes;
using GameStore.Core.Models.PlatformTypes.Specifications;
using GameStore.Core.Models.ServiceModels.PlatformTypes;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MongoDB.Bson;

namespace GameStore.Core.Services;

public class PlatformTypeService : IPlatformTypeService
{
    private readonly IMongoLogger _mongoLogger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PlatformTypeService(IMongoLogger mongoLogger, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mongoLogger = mongoLogger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private IRepository<PlatformType> Repository => _unitOfWork.GetEfRepository<PlatformType>(); 

    public async Task<ICollection<PlatformType>> GetAllAsync()
    {
        var result = await Repository.GetBySpecAsync(new PlatformTypesListSpec());

        return result;
    }

    public async Task<PlatformType> GetByIdAsync(Guid id)
    {
        var result = await Repository.GetSingleOrDefaultBySpecAsync(new PlatformTypeByIdSpec(id))
                     ?? throw new ItemNotFoundException(typeof(PlatformType), id);
        
        return result;
    }

    public async Task CreateAsync(PlatformTypeCreateModel createModel)
    {
        var platformType = _mapper.Map<PlatformType>(createModel);

        await Repository.AddAsync(platformType);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogCreateAsync(platformType);
    }

    public async Task UpdateAsync(PlatformTypeUpdateModel updateModel)
    {
        var platformType = await Repository.GetSingleOrDefaultBySpecAsync(new PlatformTypeByIdSpec(updateModel.Id))
                           ?? throw new ItemNotFoundException(typeof(PlatformType), updateModel.Id,
                                                              nameof(updateModel.Id));
        var oldPlatformVersion = platformType.ToBsonDocument();
        
        UpdateValues(updateModel, platformType);

        await Repository.UpdateAsync(platformType);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogUpdateAsync(typeof(PlatformType), oldPlatformVersion, platformType.ToBsonDocument());
    }

    public async Task DeleteAsync(Guid id)
    {
        var platformType = await Repository.GetSingleOrDefaultBySpecAsync(new PlatformTypeByIdSpec(id))
                           ?? throw new ItemNotFoundException(typeof(PlatformType), id);

        platformType.IsDeleted = true;
        await Repository.UpdateAsync(platformType);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogDeleteAsync(platformType);
    }

    private void UpdateValues(PlatformTypeUpdateModel updateModel, PlatformType platformType)
    {
        platformType.Name = updateModel.Name;
    }
}