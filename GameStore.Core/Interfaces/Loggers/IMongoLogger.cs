using System;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace GameStore.Core.Interfaces.Loggers;

public interface IMongoLogger
{
    public Task LogCreateAsync<T>(T entity);
    public Task LogUpdateAsync(Type type, BsonDocument entity, BsonDocument updated);
    public Task LogDeleteAsync<T>(T entity);
}