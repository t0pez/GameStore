using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Mongo.Orders.Specifications;

public class OrdersByDateSpec : PagedSpec<OrderMongo>
{
    public OrdersByDateSpec(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;

        Query
            .Where(order => order.OrderDate >= StartDate && order.OrderDate <= EndDate);
    }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}