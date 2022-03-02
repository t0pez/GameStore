using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.SharedKernel.Interfaces.DataAccess
{
    public interface IRepository<TModel> where TModel : BaseEntity
    {
        Task<TModel> AddAsync(TModel model);
        Task<TModel> GetByIdAsync(Guid id); // TODO: change Guid for repository and models for custom type
        Task<ICollection<TModel>> GetAllAsync();
        Task<ICollection<TModel>> GetBySpecificationAsync(ISpecification<TModel> specification);
        void Update(TModel updated);
        void Delete(TModel model);
    }
}
