using System;

namespace GameStore.Core.Models.Orders;

public class BasketItem
{
    public Guid GameId { get; set; }
    public int Quantity { get; set; }
}