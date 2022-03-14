using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.SharedKernel.Interfaces.DataAccess;

public interface IRepository<TModel> where TModel : BaseEntity
{
    Task<TModel> GetByIdAsync(Guid id);
    Task<List<TModel>> GetBySpecAsync(ISpecification<TModel> specification = null);
    Task<TModel> GetSingleBySpecAsync(ISpecification<TModel> specification);
    Task AddAsync(TModel model);
    Task UpdateAsync(TModel updated);
    Task DeleteAsync(TModel model);
    Task<bool> AnyAsync(Guid id);
    Task<bool> AnyAsync(ISpecification<TModel> specification);
}