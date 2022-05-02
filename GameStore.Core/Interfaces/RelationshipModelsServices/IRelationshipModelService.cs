using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Interfaces.RelationshipModelsServices;

public interface IRelationshipModelService<TModel> where TModel : class
{
   public Task UpdateManyToManyAsync(IEnumerable<TModel> newModels, DomainSpec<TModel> deleteSpec);
}