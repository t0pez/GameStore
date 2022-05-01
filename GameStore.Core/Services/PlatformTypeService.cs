using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.PlatformTypes;
using GameStore.Core.Models.PlatformTypes.Specifications;
using GameStore.Core.Models.ServiceModels.PlatformTypes;
using GameStore.SharedKernel.Interfaces.DataAccess;

namespace GameStore.Core.Services;

public class PlatformTypeService : IPlatformTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PlatformTypeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private IRepository<PlatformType> Repository => _unitOfWork.GetRepository<PlatformType>(); 

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
    }

    public async Task UpdateAsync(PlatformTypeUpdateModel updateModel)
    {
        var platformType = await Repository.GetSingleOrDefaultBySpecAsync(new PlatformTypeByIdSpec(updateModel.Id))
                           ?? throw new ItemNotFoundException(typeof(PlatformType), updateModel.Id,
                                                              nameof(updateModel.Id));

        UpdateValues(updateModel, platformType);

        await Repository.UpdateAsync(platformType);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var platformType = await Repository.GetSingleOrDefaultBySpecAsync(new PlatformTypeByIdSpec(id))
                           ?? throw new ItemNotFoundException(typeof(PlatformType), id);

        platformType.IsDeleted = true;
        await Repository.UpdateAsync(platformType);
        await _unitOfWork.SaveChangesAsync();
    }

    private void UpdateValues(PlatformTypeUpdateModel updateModel, PlatformType platformType)
    {
        platformType.Name = updateModel.Name;
    }
}