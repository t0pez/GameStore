using System.Threading.Tasks;

namespace GameStore.SharedKernel.Interfaces.DataAccess;

public interface IUnitOfWork
{
    public IRepository<TModel> GetRepository<TModel>() where TModel : class;
    public Task<int> SaveChangesAsync();
}