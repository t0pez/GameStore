using System.Collections.Generic;

namespace GameStore.Web.Models.Basket;

public class BasketCookieModel
{
    public ICollection<BasketItemCookieModel> Items { get; set; }
}