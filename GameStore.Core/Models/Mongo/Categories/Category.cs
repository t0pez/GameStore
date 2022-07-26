using GameStore.Core.Models.Mongo.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Core.Models.Mongo.Categories;

[MongoCollectionName("categories")]
public class Category
{
    [BsonId]
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [NavigationId]
    [BsonElement("CategoryID")]
    public int CategoryId { get; set; }

    [BsonElement("CategoryName")]
    public string Name { get; set; }

    public string Description { get; set; }

    [BsonIgnore]
    public byte[] Picture { get; set; }
}