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
        Task<ICollection<TModel>> GetBySpecAsync(ISpecification<TModel> specification);
        Task<TModel> GetSingleBySpecAsync(ISpecification<TModel> specification);
        Task UpdateAsync(TModel updated);
        Task DeleteAsync(TModel model);
    }
}
