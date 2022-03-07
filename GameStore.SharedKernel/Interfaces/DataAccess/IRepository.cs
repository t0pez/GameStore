using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.SharedKernel.Interfaces.DataAccess
{
    public interface IRepository<TModel> where TModel : BaseEntity
    {
        Task<TModel> GetByIdAsync(Guid id); // TODO: change Guid for repository and models for custom type
        Task<List<TModel>> GetBySpecAsync(ISpecification<TModel> specification = null);
        Task<TModel> GetSingleBySpecAsync(ISpecification<TModel> specification);
        Task AddAsync(TModel model);
        void Update(TModel updated);
        void Delete(TModel model);
        Task<bool> AnyAsync(Guid id);
        Task<bool> AnyAsync(ISpecification<TModel> specification);
    }
}
