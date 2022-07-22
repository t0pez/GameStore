using Ardalis.Specification;

namespace GameStore.Core.Models.Orders.Specifications;

public class OrderInProcessByCustomerIdWithDetailsSpec : OrderInProcessByCustomerIdSpec
{
    public OrderInProcessByCustomerIdWithDetailsSpec(string customerId) : base(customerId)
    {
        Query
            .Include(order => order.OrderDetails);
    }
}