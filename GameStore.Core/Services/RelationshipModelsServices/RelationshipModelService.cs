using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.Specification;
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

    public async Task DeleteBySpecAsync(ISpecification<TModel> specification)
    {
        var specificationResult = await Repository.GetBySpecAsync(specification);

        foreach (var model in specificationResult) 
            await Repository.DeleteAsync(model);
    }
}

public interface IRelationshipModelService<TModel> where TModel : RelationshipModel
{
    public Task AddAsync(TModel model);
    public Task AddRangeAsync(IEnumerable<TModel> models);
    public Task DeleteAsync(TModel model);
    public Task DeleteBySpecAsync(ISpecification<TModel> specification);
}

