using System;
using System.Collections.Generic;

namespace GameStore.Core.Models.Baskets;

public class Basket
{
    public Guid CustomerId { get; set; }
    public ICollection<BasketItem> Items { get; set; }
}