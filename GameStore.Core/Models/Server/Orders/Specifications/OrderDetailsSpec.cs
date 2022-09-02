using Ardalis.Specification;
using SpecificationExtensions.Core.Specifications;

namespace GameStore.Core.Models.Server.Orders.Specifications;

public class OrderDetailsSpec : BaseSpec<OrderDetails>
{
    public OrderDetailsSpec ByGameKey(string gameKey)
    {
        Query
           .Where(details => details.GameKey == gameKey);

        return this;
    }
}