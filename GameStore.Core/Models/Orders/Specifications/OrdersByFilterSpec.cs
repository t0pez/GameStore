using Ardalis.Specification;
using GameStore.Core.Models.Orders.Filters;
using SpecificationExtensions.Core.Specifications;

namespace GameStore.Core.Models.Orders.Specifications;

public class OrdersByFilterSpec : BaseSpec<Order>
{
    public OrdersByFilterSpec(OrdersFilter filter)
    {
        Filter = filter;

        if (filter.StartDate.HasValue)
        {
            Query
                .Where(order => order.OrderDate >= filter.StartDate);
        }

        if (filter.EndDate.HasValue)
        {
            Query
                .Where(order => order.OrderDate >= filter.EndDate);
        }

        Query
            .Include(order => order.OrderDetails);
    }

    public OrdersFilter Filter { get; set; }
}