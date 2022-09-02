using System;

namespace GameStore.Core.Models.Mongo.Orders.Filters;

public class MongoOrdersFilter
{
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}