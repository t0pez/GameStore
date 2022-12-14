using System;
using GameStore.Core.Models.Mongo.Attributes;
using MongoDB.Bson;

namespace GameStore.Core.Models.Mongo.MongoLogs;

[MongoCollectionName("logs")]
public class MongoLog
{
    public string EntityType { get; set; }

    public DateTime LogDate { get; set; }

    public LogOperation Action { get; set; }

    public BsonDocument OldObject { get; set; }

    public BsonDocument NewObject { get; set; }
}

public enum LogOperation
{
    Create,
    Update,
    Delete
}