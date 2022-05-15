using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Orders.Specifications;

public class OrderByIdSpec : SafeDeleteSpec<Order>
{
    public OrderByIdSpec(Guid id)
    {
        Id = id;

        Query
            .Where(order => order.Id == id);
    }

    public Guid Id { get; }
}