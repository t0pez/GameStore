using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.Specification;
using GameStore.Core.Interfaces.RelationshipModelsServices;
using GameStore.SharedKernel;
using GameStore.SharedKernel.Interfaces.DataAccess;

namespace GameStore.Core.Services.RelationshipModelsServices;

public class RelationshipModelService<TModel> : IRelationshipModelService<TModel> where TModel : RelationshipModel
{
    private readonly IUnitOfWork _unitOfWork;

    public RelationshipModelService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private IRepository<TModel> Repository => _unitOfWork.GetRepository<TModel>();
    
    public Task AddAsync(TModel model)
    {
        return Repository.AddAsync(model);
    }

    public Task AddRangeAsync(IEnumerable<TModel> models)
    {
        return Repository.AddRangeAsync(models);
    }

    public Task DeleteAsync(TModel model)
    {
        return Repository.DeleteAsync(model);
    }

    public async Task DeleteBySpecAsync(ISpecification<TModel> spec)
    {
        var specificationResult = await Repository.GetBySpecAsync(spec);

        await Repository.DeleteRangeAsync(specificationResult);
    }
}