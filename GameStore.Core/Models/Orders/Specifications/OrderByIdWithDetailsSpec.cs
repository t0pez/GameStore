using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Orders.Specifications;

public class OrderByIdWithDetailsSpec : OrderByIdSpec
{
    public OrderByIdWithDetailsSpec(Guid id) : base(id)
    {
        Query
            .Include(order => order.OrderDetails);
    }
}