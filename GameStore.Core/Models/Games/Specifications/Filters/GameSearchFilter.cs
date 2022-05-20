using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GameStore.SharedKernel;
using GameStore.SharedKernel.Specifications.Filters;

namespace GameStore.Core.Models.Games.Specifications.Filters;

public class GameSearchFilter : PaginationFilter
{
    public string Name { get; set; }
    public Range<decimal> PriceRange { get; set; }
    public Guid? PublisherId { get; set; }

    public IEnumerable<Guid> GenresIds { get; set; }
    public IEnumerable<Guid> PlatformsIds { get; set; }

    public Expression<Func<Game, object>> OrderBy { get; set; } = game => game.Price;
}