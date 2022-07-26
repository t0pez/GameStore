using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Orders.Specifications;

public class OrderInBasketByCustomerIdWithDetailsSpec : SafeDeleteSpec<Order>
{
    public OrderInBasketByCustomerIdWithDetailsSpec(string customerId)
    {
        CustomerId = customerId;

        Query
            .Where(order => order.CustomerId == customerId)
            .Where(order => order.Status == OrderStatus.Created ||
                            order.Status == OrderStatus.Cancelled)
            .Include(order => order.OrderDetails);
    }

    public string CustomerId { get; set; }
}