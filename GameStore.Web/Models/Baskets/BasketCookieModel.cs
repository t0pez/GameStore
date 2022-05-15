using System.Collections.Generic;

namespace GameStore.Web.Models.Baskets;

public class BasketCookieModel
{
    public ICollection<BasketItemCookieModel> Items { get; set; } = new List<BasketItemCookieModel>();
}