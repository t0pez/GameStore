using System;
using System.Collections.Generic;
using GameStore.SharedKernel.Specifications.Filters;

namespace GameStore.Core.Models.Games.Specifications.Filters;

public class GameSearchFilter : PaginationFilter
{
    public string Name { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public IEnumerable<Guid> GenresIds { get; set; } = new List<Guid>();
    public IEnumerable<Guid> PlatformsIds { get; set; } = new List<Guid>();
    public IEnumerable<Guid> PublishersIds { get; set; } = new List<Guid>();

    public GameSearchFilterOrderByState OrderBy { get; set; }
}