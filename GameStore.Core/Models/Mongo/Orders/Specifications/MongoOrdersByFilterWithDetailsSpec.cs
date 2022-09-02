using System;
using Ardalis.Specification;
using GameStore.Core.Models.Mongo.Orders.Filters;
using SpecificationExtensions.Core.Specifications;

namespace GameStore.Core.Models.Mongo.Orders.Specifications;

public class MongoOrdersByFilterWithDetailsSpec : BaseSpec<OrderMongo>
{
    public MongoOrdersByFilterWithDetailsSpec(MongoOrdersFilter filter)
    {
        Filter = filter;

        if (filter.StartDate.HasValue)
        {
            var startDateAsUtc = DateTime.SpecifyKind(filter.StartDate.Value, DateTimeKind.Utc);

            Query
                .Where(order => order.OrderDate >= startDateAsUtc);
        }

        if (filter.EndDate.HasValue)
        {
            var endDateAsUtc = DateTime.SpecifyKind(filter.EndDate.Value, DateTimeKind.Utc);

            Query
                .Where(order => order.OrderDate >= endDateAsUtc);
        }

        Query
            .Include(mongo => mongo.OrderDetails);
    }

    public MongoOrdersFilter Filter { get; set; }
}