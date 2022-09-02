using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.Loggers;
using GameStore.Core.Models.Server.PlatformTypes;
using GameStore.Core.Models.Server.PlatformTypes.Specifications;
using GameStore.Core.Models.ServiceModels.PlatformTypes;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MongoDB.Bson;

namespace GameStore.Core.Services;

public class PlatformTypeService : IPlatformTypeService
{
    private readonly IMapper _mapper;
    private readonly IMongoLogger _mongoLogger;
    private readonly IUnitOfWork _unitOfWork;

    public PlatformTypeService(IMongoLogger mongoLogger, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mongoLogger = mongoLogger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private IRepository<PlatformType> Repository => _unitOfWork.GetEfRepository<PlatformType>();

    public async Task<ICollection<PlatformType>> GetAllAsync()
    {
        var result = await Repository.GetBySpecAsync(new PlatformTypesSpec());

        return result;
    }

    public async Task<PlatformType> GetByIdAsync(Guid id)
    {
        var spec = new PlatformTypesSpec().ById(id);

        var result = await Repository.GetSingleOrDefaultBySpecAsync(spec)
                     ?? throw new ItemNotFoundException(typeof(PlatformType), id);

        return result;
    }

    public async Task CreateAsync(PlatformTypeCreateModel createModel)
    {
        var platform = _mapper.Map<PlatformType>(createModel);

        await Repository.AddAsync(platform);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogCreateAsync(platform);
    }

    public async Task UpdateAsync(PlatformTypeUpdateModel updateModel)
    {
        var spec = new PlatformTypesSpec().ById(updateModel.Id);

        var platform = await Repository.GetSingleOrDefaultBySpecAsync(spec)
                       ?? throw new ItemNotFoundException(typeof(PlatformType), updateModel.Id,
                                                          nameof(updateModel.Id));

        var oldPlatformVersion = platform.ToBsonDocument();

        UpdateValues(platform, updateModel);

        await Repository.UpdateAsync(platform);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogUpdateAsync(typeof(PlatformType), oldPlatformVersion, platform.ToBsonDocument());
    }

    public async Task DeleteAsync(Guid id)
    {
        var spec = new PlatformTypesSpec().ById(id);

        var platform = await Repository.GetSingleOrDefaultBySpecAsync(spec)
                       ?? throw new ItemNotFoundException(typeof(PlatformType), id);

        platform.IsDeleted = true;
        await Repository.UpdateAsync(platform);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogDeleteAsync(platform);
    }

    private void UpdateValues(PlatformType platform, PlatformTypeUpdateModel updateModel)
    {
        platform.Name = updateModel.Name;
    }
}