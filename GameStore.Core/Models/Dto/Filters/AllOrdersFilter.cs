using System;

namespace GameStore.Core.Models.Dto.Filters;

public class AllOrdersFilter
{
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}