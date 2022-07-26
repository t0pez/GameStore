using System;
using System.Threading.Tasks;
using GameStore.Core.Interfaces.Loggers;
using GameStore.Core.Models.Mongo.MongoLogs;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MongoDB.Bson;

namespace GameStore.Core.Services.Loggers;

public class MongoLogger : IMongoLogger
{
    private readonly IUnitOfWork _unitOfWork;

    public MongoLogger(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private IRepository<MongoLog> LogsRepository => _unitOfWork.GetMongoRepository<MongoLog>();

    public async Task LogCreateAsync<T>(T entity)
    {
        var versionOfObject = entity.ToBsonDocument();

        await LogAsync(typeof(T), LogOperation.Create, versionOfObject);
    }

    public async Task LogUpdateAsync(Type type, BsonDocument entity, BsonDocument updated)
    {
        await LogAsync(type, LogOperation.Update, entity, updated);
    }

    public async Task LogDeleteAsync<T>(T entity)
    {
        var versionOfObject = entity.ToBsonDocument();

        await LogAsync(typeof(T), LogOperation.Delete, versionOfObject);
    }

    private async Task LogAsync(Type type, LogOperation action, BsonDocument versionOfObject,
                                BsonDocument updatedObject = null)
    {
        var log = new MongoLog
        {
            EntityType = type.Name,
            LogDate = DateTime.UtcNow,
            Action = action,
            VersionOfObject = versionOfObject
        };

        await LogsRepository.AddAsync(log);
    }
}