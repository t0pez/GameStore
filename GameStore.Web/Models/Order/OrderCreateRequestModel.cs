using GameStore.Core.Models.Baskets;

namespace GameStore.Web.Models.Order;

public class OrderCreateRequestModel
{
    public Basket Basket { get; set; }
}