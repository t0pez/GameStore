using System.Threading;
using System.Threading.Tasks;

namespace GameStore.SharedKernel.Interfaces.DataAccess;

public interface IUnitOfWork
{
    IRepository<TModel> GetRepository<TModel>() where TModel : BaseEntity;
    Task<int> SaveChangesAsync();
}