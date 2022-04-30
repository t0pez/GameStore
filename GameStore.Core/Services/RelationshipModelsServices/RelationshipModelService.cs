using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.Specification;
using GameStore.Core.Interfaces.RelationshipModelsServices;
using GameStore.SharedKernel.Interfaces.DataAccess;

namespace GameStore.Core.Services.RelationshipModelsServices;

public class RelationshipModelService<TModel> : IRelationshipModelService<TModel> where TModel : class
{
    private readonly IUnitOfWork _unitOfWork;

    public RelationshipModelService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private IRepository<TModel> Repository => _unitOfWork.GetRepository<TModel>();
    
    public async Task UpdateManyToManyAsync(IEnumerable<TModel> newModels, ISpecification<TModel> deleteSpec)
    {
        await DeleteByPreviousRelationshipsAsync(deleteSpec);
        await AddNewRelationshipsAsync(newModels);
    }

    private Task AddNewRelationshipsAsync(IEnumerable<TModel> models)
    {
        return Repository.AddRangeAsync(models);
    }

    private async Task DeleteByPreviousRelationshipsAsync(ISpecification<TModel> spec)
    {
        var specificationResult = await Repository.GetBySpecAsync(spec);

        await Repository.DeleteRangeAsync(specificationResult);
    }
}