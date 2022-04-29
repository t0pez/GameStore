using GameStore.Core.Models.Games;

namespace GameStore.Core.Models.Baskets;

public class BasketItem
{
    public Game Game { get; set; }
    public int Quantity { get; set; }
}