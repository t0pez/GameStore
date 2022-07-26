using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.Specification;

namespace GameStore.Core.Interfaces.RelationshipModelsServices;

public interface IRelationshipModelService<TModel> where TModel : class
{
    public Task UpdateManyToManyAsync(IEnumerable<TModel> newModels, ISpecification<TModel> deleteSpec);
}