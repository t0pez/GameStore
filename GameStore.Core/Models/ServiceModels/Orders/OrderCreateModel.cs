using GameStore.Core.Models.Baskets;

namespace GameStore.Core.Models.ServiceModels.Orders;

public class OrderCreateModel
{
    public Basket Basket { get; set; }
}