using Ardalis.Specification;

namespace GameStore.Core.Models.Orders.Specifications;

public class OrderDetailsByGameKeySpec : Specification<OrderDetails>
{
    public OrderDetailsByGameKeySpec(string gameKey)
    {
        GameKey = gameKey;

        Query
            .Where(details => details.GameKey == gameKey);
    }

    public string GameKey { get; set; }
}