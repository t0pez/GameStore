using GameStore.Core.Models.Mongo.Attributes;
using GameStore.Core.Models.Mongo.Products;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Core.Models.Mongo.Orders;

[MongoCollectionName("order-details")]
public class OrderDetailsMongo
{
    [BsonId]
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public int Quantity { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Discount { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal UnitPrice { get; set; }

    [BsonElement("ProductID")]
    public int ProductId { get; set; }

    [NavigationProperty(nameof(ProductId))]
    public Product Product { get; set; }

    [BsonElement("OrderID")]
    public int OrderId { get; set; }

    [NavigationProperty(nameof(OrderId))]
    public OrderMongo Order { get; set; }

    public decimal TotalPrice => UnitPrice * Quantity - UnitPrice * Quantity * Discount;
}