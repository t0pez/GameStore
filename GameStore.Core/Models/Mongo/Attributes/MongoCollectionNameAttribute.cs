using System;

namespace GameStore.Core.Models.Mongo.Attributes;

public class MongoCollectionNameAttribute : Attribute
{
    public MongoCollectionNameAttribute(string collectionName)
    {
        CollectionName = collectionName;
    }

    public string CollectionName { get; }
}