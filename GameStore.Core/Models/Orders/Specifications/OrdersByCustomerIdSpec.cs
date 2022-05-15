using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Orders.Specifications;

public class OrdersByCustomerIdSpec : Specification<Order>
{
    public OrdersByCustomerIdSpec(Guid customerId)
    {
        CustomerId = customerId;

        Query
            .Where(order => order.CustomerId == customerId);
    }

    public Guid CustomerId { get; }
}