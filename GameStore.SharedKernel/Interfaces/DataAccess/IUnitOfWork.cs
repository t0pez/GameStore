using System.Threading.Tasks;

namespace GameStore.SharedKernel.Interfaces.DataAccess;

public interface IUnitOfWork
{
    public IRepository<TModel> GetEfRepository<TModel>() where TModel : class;
    public IRepository<TModel> GetMongoRepository<TModel>() where TModel : class;
    public Task<int> SaveChangesAsync();
}