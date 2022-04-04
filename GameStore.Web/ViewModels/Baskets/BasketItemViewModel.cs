using System;

namespace GameStore.Web.ViewModels.Baskets;

public class BasketItemViewModel
{
    public Guid GameId { get; set; }
    public int Quantity { get; set; }
}