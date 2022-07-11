using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper.Execution;
using AutoMapper.Internal;
using GameStore.Core.Models.Mongo.Attributes;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace GameStore.Infrastructure.Data.Repositories;

public class MongoRepository<T> : IRepository<T> where T : class
{
    private readonly IMongoDatabase _database;

    public MongoRepository(IMongoDatabase database)
    {
        _database = database;
    }

    private IMongoCollection<T> Collection => _database.GetNorthwindCollection<T>();

    public Task<List<T>> GetBySpecAsync(ISpecification<T> spec = null)
    {
        if (spec is null)
        {
            return Collection.Find(Builders<T>.Filter.Empty).ToListAsync();
        }

        var query = ApplySpecification(Collection.AsQueryable(), spec);

        return query.ToListAsync();
    }

    public async Task<T> GetFirstOrDefaultBySpecAsync(ISpecification<T> spec)
    {
        var query = ApplySpecification(Collection.AsQueryable(), spec);

        var model = await query.FirstOrDefaultAsync();

        if (model is not null)
        {
            AddIncludeProperties(model, spec);
        }

        return model;
    }

    public async Task<T> GetSingleOrDefaultBySpecAsync(ISpecification<T> spec)
    {
        var query = ApplySpecification(Collection.AsQueryable(), spec);

        var model = await query.SingleOrDefaultAsync();

        if (model is not null)
        {
            AddIncludeProperties(model, spec);
        }

        return model;
    }

    public Task AddAsync(T model)
    {
        return Collection.InsertOneAsync(model);
    }

    public Task AddRangeAsync(IEnumerable<T> models)
    {
        return Collection.InsertManyAsync(models);
    }

    public Task UpdateAsync(T updated)
    {
        var filter = Builders<T>.Filter.GetFilterDefinition(updated);

        var updateBuilder = Builders<T>.Update;
        UpdateDefinition<T> updateDefinition = null;

        var existEntity = Collection.Find(filter).SingleOrDefault();

        var entityType = updated.GetType();

        foreach (var propertyInfo in entityType.GetProperties())
        {
            var existEntityProperty = propertyInfo.GetValue(existEntity);
            var updatingEntityProperty = propertyInfo.GetValue(updated);

            if (updatingEntityProperty is null || updatingEntityProperty.Equals(existEntityProperty))
            {
                continue;
            }

            var expressionParameter = Expression.Parameter(entityType);
            var expressionProperty = Expression.Property(expressionParameter, propertyInfo);

            Expression conversion = Expression.Convert(expressionProperty, typeof(object));
            var expression = Expression.Lambda<Func<T, object>>(conversion, expressionParameter);

            updateDefinition = updateDefinition != null
                ? updateDefinition.Set(expression, updatingEntityProperty)
                : updateBuilder.Set(expression, updatingEntityProperty);
        }

        return updateDefinition is not null ? Collection.UpdateOneAsync(filter, updateDefinition) : Task.CompletedTask;
    }

    public Task DeleteAsync(T model)
    {
        var filterDefinition = Builders<T>.Filter.GetFilterDefinition(model);

        return Collection.DeleteOneAsync(filterDefinition);
    }

    public Task DeleteRangeAsync(IEnumerable<T> models)
    {
        var filters = models.Select(model => Builders<T>.Filter.GetFilterDefinition(model));
        var filter = Builders<T>.Filter.Or(filters);

        return Collection.DeleteManyAsync(filter);
    }

    public Task<bool> AnyAsync(ISpecification<T> spec)
    {
        var query = ApplySpecification(Collection.AsQueryable(), spec);

        return query.AnyAsync();
    }

    public Task<int> CountAsync(ISpecification<T> spec = null)
    {
        var query = ApplySpecification(Collection.AsQueryable(), spec);

        return query.CountAsync();
    }

    private IMongoQueryable<T> ApplySpecification(IQueryable<T> query, ISpecification<T> spec)
    {
        if (spec is null)
        {
            return (IMongoQueryable<T>)query;
        }

        var evaluator = new SpecificationEvaluator(true);
        var specResult = (IMongoQueryable<T>)evaluator.GetQuery(query, spec, true);

        return specResult;
    }

    public void AddIncludeProperties(T model, ISpecification<T> spec)
    {
        if (spec.IncludeExpressions.Any() == false)
        {
            return;
        }

        var singleIncludeExpressions =
            spec.IncludeExpressions
                .Where(expression => expression.PropertyType.IsCollection() == false)
                .Select(info => info.LambdaExpression);
        var multiIncludeExpressions =
            spec.IncludeExpressions
                .Where(expression => expression.PropertyType.IsCollection())
                .Select(info => info.LambdaExpression);

        var singleIncludeStrings = singleIncludeExpressions
            .Select(includeExpression =>
                        includeExpression.Body.GetMemberExpressions().First().Member.Name).ToList();
        var multiIncludeStrings = multiIncludeExpressions
            .Select(includeExpression =>
                        includeExpression.Body.GetMemberExpressions().First().Member.Name).ToList();

        if (singleIncludeStrings.Any())
        {
            var singleNavProps = typeof(T).GetProperties().Where(propInfo => singleIncludeStrings.Contains(propInfo.Name));
            AddSingleIncludeProperties(model, singleNavProps);
        }

        if (multiIncludeStrings.Any())
        {
            var multiNavProps = typeof(T).GetProperties().Where(propInfo => multiIncludeStrings.Contains(propInfo.Name));
            AddManyIncludeProperties(model, multiNavProps);
        }
    }

    private void AddSingleIncludeProperties(T model, IEnumerable<PropertyInfo> navProps)
    {
        var sourceEntityType = typeof(T);

        foreach (var navProp in navProps)
        {
            var targetEntityType = navProp.PropertyType;
            var targetNavIdPropName = navProp.GetCustomAttribute<NavigationPropertyAttribute>()?.NavigationIdName;

            var sourceNavIdValue = sourceEntityType.GetProperty(targetNavIdPropName)?.GetValue(model);
            var targetNavIdProperty = targetEntityType.GetProperties()
                                                      .Single(info => info.GetCustomAttribute<NavigationIdAttribute>()
                                                                  is not null);

            var mongoCollectionName =
                targetEntityType.GetCustomAttribute<MongoCollectionNameAttribute>()?.CollectionName;
            var collection = GetMongoCollection(targetEntityType, mongoCollectionName);

            var query = GetQuery(targetEntityType, collection);

            var lambda = GetFilterExpression(targetEntityType, targetNavIdProperty, sourceNavIdValue);

            var firstOrDefaultMethod = typeof(Queryable).GetMethods()
                                                        .First(info => info.Name == "FirstOrDefault");
            var firstOrDefaultGenericMethod = firstOrDefaultMethod.MakeGenericMethod(targetEntityType);
            var navEntity = firstOrDefaultGenericMethod.Invoke(null, new[] { query, lambda });

            navProp.SetValue(model, navEntity);
        }
    }

    private void AddManyIncludeProperties(T model, IEnumerable<PropertyInfo> navProps)
    {
        var sourceEntityType = typeof(T);

        foreach (var navProp in navProps)
        {
            var targetEntityType = navProp.PropertyType.GenericTypeArguments.First();

            var targetNavIdPropName = navProp.GetCustomAttribute<ManyNavigationPropertyAttribute>()?.NavigationIdName;

            var sourceNavIdValue = sourceEntityType.GetProperties()
                                                   .Single(info =>
                                                               info.GetCustomAttribute<NavigationIdAttribute>() is not
                                                                   null)
                                                   .GetValue(model);
            var navIdProperty = targetEntityType.GetProperty(targetNavIdPropName);

            var mongoCollectionName =
                targetEntityType.GetCustomAttribute<MongoCollectionNameAttribute>()?.CollectionName;
            var collection = GetMongoCollection(targetEntityType, mongoCollectionName);

            var query = GetQuery(targetEntityType, collection);

            var lambda = GetFilterExpression(targetEntityType, navIdProperty, sourceNavIdValue);

            var whereMethod = typeof(Queryable).GetMethods()
                                               .First(info => info.Name == "Where");
            var whereGenericMethod = whereMethod.MakeGenericMethod(targetEntityType);
            var navEntities = whereGenericMethod.Invoke(null, new[] { query, lambda });

            var toListMethod =
                typeof(IAsyncCursorSourceExtensions).GetMethod(nameof(IAsyncCursorSourceExtensions.ToList));
            var toListGenericMethod = toListMethod?.MakeGenericMethod(targetEntityType);
            var mapped = toListGenericMethod?.Invoke(_database, new[] { navEntities, null });

            navProp.SetValue(model, mapped);
        }
    }

    private object GetMongoCollection(Type targetEntityType, string mongoCollectionName)
    {
        var getCollectionMethod = typeof(MongoDatabaseBase).GetMethod(nameof(MongoDatabaseBase.GetCollection));
        var getCollectionGenericMethod = getCollectionMethod?.MakeGenericMethod(targetEntityType);
        var collection = getCollectionGenericMethod?.Invoke(_database, new object[] { mongoCollectionName, null });
        
        return collection;
    }

    private static object GetQuery(Type targetEntityType, object mongoCollection)
    {
        var asQueryableMethod = typeof(IMongoCollectionExtensions).GetMethods()
                                                                  .First(info => info.Name == "AsQueryable" &&
                                                                             info.GetParameters().Length == 2);
        var asQueryableGenericMethod = asQueryableMethod.MakeGenericMethod(targetEntityType);
        var query = asQueryableGenericMethod.Invoke(null, new[] { mongoCollection, null });
        
        return query;
    }

    private static LambdaExpression GetFilterExpression(Type targetEntityType, PropertyInfo targetNavIdProperty,
                                                        object sourceNavIdValue)
    {
        var parameterExpression = Expression.Parameter(targetEntityType, "navProperty");
        var navPropertyExpression = Expression.Property(parameterExpression, targetNavIdProperty);
        var navIdExpression = Expression.Constant(sourceNavIdValue);
        var equalExpression = Expression.Equal(navPropertyExpression, navIdExpression);
        var lambda = Expression.Lambda(equalExpression, parameterExpression);
        
        return lambda;
    }
}