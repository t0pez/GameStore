using System;
using GameStore.Core.Models.Games;

namespace GameStore.Core.Models.Orders;

public class OrderDetails
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    
    public Guid GameId { get; set; }
    public Game Game { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; }
}