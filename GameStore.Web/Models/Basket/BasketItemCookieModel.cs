using System;

namespace GameStore.Web.Models.Basket;

public class BasketItemCookieModel
{
    public Guid GameId { get; set; }
    public int Quantity { get; set; }
}