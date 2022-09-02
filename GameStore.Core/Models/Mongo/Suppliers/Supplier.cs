using GameStore.Core.Models.Mongo.Attributes;
using GameStore.Core.Models.ServiceModels.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Core.Models.Mongo.Suppliers;

[MongoCollectionName("suppliers")]
public class Supplier
{
    [BsonId]
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [NavigationId]
    [BsonElement("SupplierID")]
    public int SupplierId { get; set; }

    public string CompanyName { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string HomePage { get; set; }

    public string ContactName { get; set; }

    public string ContactTitle { get; set; }

    public string Phone { get; set; }

    public string Fax { get; set; }

    public string Country { get; set; }

    public string PostalCode { get; set; }

    public string Region { get; set; }

    [BsonIgnore]
    public Database Database { get; set; } = Database.Mongo;
}