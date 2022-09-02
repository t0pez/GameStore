using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Server.Orders.Specifications;

public class OrdersSpec : SafeDeleteSpec<Order>
{
    public OrdersSpec ById(Guid id)
    {
        Query
           .Where(order => order.Id == id);

        return this;
    }

    public OrdersSpec ByCustomerId(Guid customerId)
    {
        Query
           .Where(order => order.CustomerId == customerId);

        return this;
    }

    public OrdersSpec InBasket()
    {
        Query
           .Where(order => order.Status == OrderStatus.Created ||
                           order.Status == OrderStatus.Cancelled);

        return this;
    }

    public OrdersSpec InProcess()
    {
        Query
           .Where(order => order.Status == OrderStatus.InProcess);

        return this;
    }

    public OrdersSpec WithDetails()
    {
        Query
           .Include(order => order.OrderDetails);

        return this;
    }
}