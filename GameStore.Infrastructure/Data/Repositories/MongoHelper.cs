using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GameStore.Core.Models.Mongo.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace GameStore.Infrastructure.Data.Repositories;

public static class MongoHelper
{
    public static IMongoCollection<T> GetNorthwindCollection<T>(this IMongoDatabase database)
    {
        var type = typeof(T);

        var mongoCollectionNameAttribute = type.GetCustomAttribute<MongoCollectionNameAttribute>()
                                           ?? throw new ArgumentException(
                                               "Model doesnt have MongoCollectionNameAttribute");

        var collectionName = mongoCollectionNameAttribute.CollectionName;
        var mongoCollection = database.GetCollection<T>(collectionName);

        return mongoCollection;
    }

    public static FilterDefinition<T> GetFilterDefinition<T>(this FilterDefinitionBuilder<T> builder, T model)
    {
        var type = model.GetType();
        var properties = type.GetProperties();

        var property = properties.SingleOrDefault(info => info.GetCustomAttribute(typeof(BsonIdAttribute)) is not null)
                       ?? throw new ArgumentException("Argument must have an BsonId Attribute");

        var param = Expression.Parameter(type);
        var field = Expression.Property(param, property);
        var func = Expression.Lambda<Func<T, object>>(field, param);

        var propertyValue = property.GetValue(model);

        var filter = builder.Eq(func, propertyValue);

        return filter;
    }
}