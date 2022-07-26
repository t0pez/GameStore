using Ardalis.Specification;

namespace GameStore.Core.Models.Orders.Specifications;

public class OrderInProcessByCustomerIdSpec : OrdersByCustomerIdSpec
{
    public OrderInProcessByCustomerIdSpec(string customerId) : base(customerId)
    {
        Query
            .Where(order => order.Status == OrderStatus.InProcess);
    }
}