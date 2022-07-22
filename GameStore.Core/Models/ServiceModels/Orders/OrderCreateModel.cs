using System.Collections.Generic;
using GameStore.Core.Models.Orders;

namespace GameStore.Core.Models.ServiceModels.Orders;

public class OrderCreateModel
{
    public string CustomerId { get; set; }
    public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();
}