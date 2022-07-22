using GameStore.Core.Models.Mongo.Attributes;
using GameStore.Core.Models.Mongo.Categories;
using GameStore.Core.Models.Mongo.Suppliers;
using GameStore.Core.Models.ServiceModels.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Core.Models.Mongo.Products;

[MongoCollectionName("products")]
public class Product
{
    [BsonId]
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [NavigationId]
    [BsonElement("ProductID")]
    public int ProductId { get; set; }

    [UpdatableProperty]
    public string GameKey { get; set; }
    
    public string ProductName { get; set; }
    public string QuantityPerUnit { get; set; }
    
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal UnitPrice { get; set; }
    
    public bool Discontinued { get; set; }
    
    [UpdatableProperty]
    public int UnitsInStock { get; set; }
    
    [UpdatableProperty]
    public int Views { get; set; }

    [BsonElement("CategoryID")]
    public int CategoryId { get; set; }

    [NavigationProperty(nameof(CategoryId))]
    public Category Category { get; set; }

    [BsonElement("SupplierID")] 
    public int SupplierId { get; set; }
    
    [NavigationProperty(nameof(SupplierId))]
    public Supplier Supplier { get; set; }

    [BsonIgnore]
    public Database Database { get; set; } = Database.Mongo;
}