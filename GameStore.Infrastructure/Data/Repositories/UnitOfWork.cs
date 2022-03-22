using GameStore.Infrastructure.Data.Context;
using GameStore.SharedKernel.Interfaces.DataAccess;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.Infrastructure.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _context;
    private readonly Dictionary<Type, object> _repositories;

    public UnitOfWork(ApplicationContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
    }

    public IRepository<TModel> GetRepository<TModel>() where TModel : class
    {
        var modelType = typeof(TModel);

        if (HasRepositoryOfModelType(modelType) == false) {
            if (_context.Set<TModel>() is null)
                throw new InvalidOperationException($"Context doesn't have DbSet of this type {modelType}");

            _repositories.Add(modelType, new Repository<TModel>(_context));
        }

        return (IRepository<TModel>)_repositories[modelType];
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    private bool HasRepositoryOfModelType(Type type)
    {
        return _repositories.ContainsKey(type);
    }
}