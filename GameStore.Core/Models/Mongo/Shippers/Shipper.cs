using GameStore.Core.Models.Mongo.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Core.Models.Mongo.Shippers;

[MongoCollectionName("shippers")]
public class Shipper
{
    [BsonElement("_id")]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [NavigationId]
    [BsonElement("ShipperID")]
    public int ShipperId { get; set; }

    public string CompanyName { get; set; }
    public string Phone { get; set; }
}