using System.Threading.Tasks;

namespace GameStore.SharedKernel.Interfaces.DataAccess;

public interface IUnitOfWork
{
    IRepository<TModel> GetRepository<TModel>() where TModel : class;
    Task<int> SaveChangesAsync();
}