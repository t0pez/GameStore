using System;

namespace GameStore.Web.Models.Order;

public class AllOrdersFilterRequestModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}