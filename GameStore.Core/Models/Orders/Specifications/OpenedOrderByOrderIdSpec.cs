using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Orders.Specifications;

public class OpenedOrderByOrderIdSpec : Specification<OpenedOrder>
{
    public OpenedOrderByOrderIdSpec(Guid orderId)
    {
        OrderId = orderId;

        Query
            .Where(openedOrder => openedOrder.OrderId == orderId);
    }

    public Guid OrderId { get; set; }
}