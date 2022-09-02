using System;
using Ardalis.Specification;
using SpecificationExtensions.Core.Specifications;

namespace GameStore.Core.Models.Server.Orders.Specifications;

public class OpenedOrdersSpec : BaseSpec<OpenedOrder>
{
    public OpenedOrdersSpec ByOrderId(Guid orderId)
    {
        Query
           .Where(openedOrder => openedOrder.OrderId == orderId);

        return this;
    }
}