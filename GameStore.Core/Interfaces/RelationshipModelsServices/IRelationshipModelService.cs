using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.Specification;
using GameStore.SharedKernel;

namespace GameStore.Core.Interfaces.RelationshipModelsServices;

public interface IRelationshipModelService<TModel> where TModel : RelationshipModel
{
    public Task AddAsync(TModel model);
    public Task AddRangeAsync(IEnumerable<TModel> models);
    public Task DeleteAsync(TModel model);
    public Task DeleteBySpecAsync(ISpecification<TModel> spec);
}