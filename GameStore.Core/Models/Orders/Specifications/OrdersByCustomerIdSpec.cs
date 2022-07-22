using Ardalis.Specification;

namespace GameStore.Core.Models.Orders.Specifications;

public class OrdersByCustomerIdSpec : Specification<Order>
{
    public OrdersByCustomerIdSpec(string customerId)
    {
        CustomerId = customerId;

        Query
            .Where(order => order.CustomerId == customerId);
    }

    public string CustomerId { get; }
}