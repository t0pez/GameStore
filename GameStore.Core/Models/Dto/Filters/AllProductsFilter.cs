using System;
using System.Collections.Generic;
using GameStore.Core.Models.Server.Games.Filters;
using GameStore.SharedKernel.Specifications.Filters;

namespace GameStore.Core.Models.Dto.Filters;

public class AllProductsFilter : PaginationFilter
{
    public string Name { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public IEnumerable<Guid> GenresIds { get; set; } = new List<Guid>();

    public IEnumerable<string> PublishersNames { get; set; } = new List<string>();

    public IEnumerable<Guid> PlatformsIds { get; set; } = new List<Guid>();

    public GameSearchFilterPublishedAtState PublishedAtState { get; set; }

    public GameSearchFilterOrderByState OrderByState { get; set; }
}