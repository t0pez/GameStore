using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.Specification;

namespace GameStore.SharedKernel.Interfaces.DataAccess;

public interface IRepository<TModel> where TModel : class
{
    public Task<List<TModel>> GetBySpecAsync(ISpecification<TModel> spec = null);
    public Task<List<TResult>> SelectBySpecAsync<TResult>(ISpecification<TModel, TResult> spec);
    public Task<TModel> GetFirstOrDefaultBySpecAsync(ISpecification<TModel> spec);
    public Task<TModel> GetSingleOrDefaultBySpecAsync(ISpecification<TModel> spec);
    public Task AddAsync(TModel model);
    public Task AddRangeAsync(IEnumerable<TModel> models);
    public Task UpdateAsync(TModel updated);
    public Task DeleteAsync(TModel model);
    public Task DeleteRangeAsync(IEnumerable<TModel> models);
    public Task<bool> AnyAsync(ISpecification<TModel> spec);
    public Task<int> CountAsync(ISpecification<TModel> spec = null);
}