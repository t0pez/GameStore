using GameStore.Core.Models.Games;

namespace GameStore.Core.Models.Baskets;

public class BasketItem // TODO: Possibly add Discount here if it not in game
{
    public Game Game { get; set; }
    public int Quantity { get; set; }
}