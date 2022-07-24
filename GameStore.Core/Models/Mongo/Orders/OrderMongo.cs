using System;
using System.Collections.Generic;
using GameStore.Core.Models.Mongo.Attributes;
using GameStore.Core.Models.Mongo.Shippers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Core.Models.Mongo.Orders;

[MongoCollectionName("orders")]
public class OrderMongo
{
    [BsonId]
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [NavigationId]
    [BsonElement("OrderID")]
    public int OrderId { get; set; }

    public decimal Freight { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime RequiredDate { get; set; }

    public string ShipAddress { get; set; }

    public string ShipCity { get; set; }

    public string ShipCountry { get; set; }

    public string ShipName { get; set; }

    public string ShipPostalCode { get; set; }

    public string ShipRegion { get; set; }

    public DateTime ShippedDate { set; get; }

    [BsonElement("ShipVia")]
    public int ShipperId { get; set; }

    [NavigationProperty(nameof(ShipperId))]
    public Shipper Shipper { get; set; }

    [BsonElement("EmployeeID")]
    public int EmployeeId { get; set; }

    [BsonElement("CustomerID")]
    public string CustomerId { get; set; }

    [ManyNavigationProperty(nameof(OrderDetailsMongo.OrderId))]
    public List<OrderDetailsMongo> OrderDetails { get; set; }
}