using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.SharedKernel.Interfaces.DataAccess
{
    public interface IRepository<TModel> where TModel : BaseEntity
    {
        Task<TModel> AddAsync(TModel model, CancellationToken token = default);
        Task<TModel> GetByIdAsync(Guid id, CancellationToken token = default); // TODO: change Guid for repository and models for custom type
        Task<ICollection<TModel>> GetAllAsync(CancellationToken token = default);
        Task<ICollection<TModel>> GetBySpecAsync(ISpecification<TModel> specification, CancellationToken token = default);
        Task<TModel> GetSingleBySpecAsync(ISpecification<TModel> specification, CancellationToken token = default);
        Task UpdateAsync(TModel updated, CancellationToken token = default);
        Task DeleteAsync(TModel model, CancellationToken token = default);
    }
}
