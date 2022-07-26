using System;
using System.ComponentModel.DataAnnotations.Schema;
using GameStore.Core.Models.Dto;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Core.Models.Orders;

public class OrderDetails
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }

    public string GameKey { get; set; }

    [NotMapped]
    [BsonIgnore]
    public ProductDto Game { get; set; }

    public Guid OrderId { get; set; }

    [BsonIgnore]
    public Order Order { get; set; }

    public decimal TotalPrice => Price * Quantity - Discount;
}