using System;

namespace GameStore.Core.Models.Server.Orders.Filters;

public class OrdersFilter
{
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}