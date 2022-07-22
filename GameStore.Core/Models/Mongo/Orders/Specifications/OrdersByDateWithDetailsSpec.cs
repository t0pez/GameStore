using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Mongo.Orders.Specifications;

public class OrdersByDateWithDetailsSpec : OrdersByDateSpec
{
    public OrdersByDateWithDetailsSpec(DateTime startDate, DateTime endDate) : base(startDate, endDate)
    {
        Query
            .Include(order => order.OrderDetails)
            .Include(order => order.Shipper);
    }
}