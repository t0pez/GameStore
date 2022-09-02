using System;
using System.Collections.Generic;
using GameStore.Core.Models.Server.Orders;

namespace GameStore.Core.Models.ServiceModels.Orders;

public class OrderCreateModel
{
    public Guid CustomerId { get; set; }

    public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();
}