using Ardalis.Specification;
using GameStore.Core.Models.Mongo.Orders.Filters;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Mongo.Orders.Specifications;

public class MongoOrdersByFilterWithDetailsSpec : PagedSpec<OrderMongo>
{
    public MongoOrdersByFilterWithDetailsSpec(MongoOrdersFilter filter)
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
            .Include(mongo => mongo.OrderDetails);
    }

    public MongoOrdersFilter Filter { get; set; }
}