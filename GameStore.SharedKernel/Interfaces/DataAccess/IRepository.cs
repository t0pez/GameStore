using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.SharedKernel.Specifications;

namespace GameStore.SharedKernel.Interfaces.DataAccess;

public interface IRepository<TModel> where TModel : class
{
    public Task<List<TModel>> GetBySpecAsync(DomainSpec<TModel> spec = null);
    public Task<TModel> GetSingleOrDefaultBySpecAsync(DomainSpec<TModel> spec);
    public Task AddAsync(TModel model);
    public Task AddRangeAsync(IEnumerable<TModel> models);
    public Task UpdateAsync(TModel updated);
    public Task DeleteAsync(TModel model);
    public Task DeleteRangeAsync(IEnumerable<TModel> models);
    public Task<bool> AnyAsync(DomainSpec<TModel> spec);
    public Task<int> CountAsync(DomainSpec<TModel> spec = null);
}