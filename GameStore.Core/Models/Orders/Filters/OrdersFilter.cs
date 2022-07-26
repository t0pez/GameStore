using System;

namespace GameStore.Core.Models.Orders.Filters;

public class OrdersFilter
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}