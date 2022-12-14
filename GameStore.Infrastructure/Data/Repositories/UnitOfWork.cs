using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Infrastructure.Data.Context;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MongoDB.Driver;

namespace GameStore.Infrastructure.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _context;
    private readonly IMongoDatabase _database;
    private readonly Dictionary<Type, object> _efRepositories = new();
    private readonly Dictionary<Type, object> _mongoRepositories = new();

    public UnitOfWork(ApplicationContext context, string mongoConnectionString)
    {
        _context = context;
        var connection = new MongoUrlBuilder(mongoConnectionString);
        var client = new MongoClient(mongoConnectionString);
        _database = client.GetDatabase(connection.DatabaseName);
    }

    public IRepository<TModel> GetEfRepository<TModel>() where TModel : class
    {
        var modelType = typeof(TModel);

        if (HasEfRepositoryOfModelType(modelType) == false)
        {
            _efRepositories.Add(modelType, new EfRepository<TModel>(_context));
        }

        return (IRepository<TModel>)_efRepositories[modelType];
    }

    public IRepository<TModel> GetMongoRepository<TModel>() where TModel : class
    {
        var modelType = typeof(TModel);

        if (HasMongoRepositoryOfModelType(modelType) == false)
        {
            _mongoRepositories.Add(modelType, new MongoRepository<TModel>(_database));
        }

        return (IRepository<TModel>)_mongoRepositories[modelType];
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    private bool HasEfRepositoryOfModelType(Type type)
    {
        return _efRepositories.ContainsKey(type);
    }

    private bool HasMongoRepositoryOfModelType(Type type)
    {
        return _mongoRepositories.ContainsKey(type);
    }
}