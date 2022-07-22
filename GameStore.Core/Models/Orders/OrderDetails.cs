using System;
using System.ComponentModel.DataAnnotations.Schema;
using GameStore.Core.Models.Dto;

namespace GameStore.Core.Models.Orders;

public class OrderDetails
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    
    public string GameKey { get; set; }
    [NotMapped]
    public ProductDto Game { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; }
    
    public decimal TotalPrice => Price * Quantity - Discount;
}