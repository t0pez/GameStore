﻿using Ardalis.Specification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.SharedKernel.Interfaces.DataAccess;

public interface IRepository<TModel> where TModel : class
{
    Task<List<TModel>> GetBySpecAsync(ISpecification<TModel> spec = null);
    Task<TModel> GetSingleBySpecAsync(ISpecification<TModel> spec);
    Task AddAsync(TModel model);
    Task AddRangeAsync(IEnumerable<TModel> models);
    Task UpdateAsync(TModel updated);
    Task DeleteAsync(TModel model);
    Task DeleteRangeAsync(IEnumerable<TModel> models);
    Task<bool> AnyAsync(ISpecification<TModel> spec);
}