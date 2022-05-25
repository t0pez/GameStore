using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GameStore.SharedKernel.Specifications.Filters;
using SimpleValueRange;

namespace GameStore.Core.Models.Games.Specifications.Filters;

public class GameSearchFilter : PaginationFilter
{
    public string Name { get; set; }
    public Range<decimal> PriceRange { get; set; }

    public IEnumerable<Guid> GenresIds { get; set; } = new List<Guid>();
    public IEnumerable<Guid> PlatformsIds { get; set; } = new List<Guid>();
    public IEnumerable<Guid> PublishersIds { get; set; } = new List<Guid>();

    public Expression<Func<Game, object>> OrderBy { get; set; } = game => game.Comments.Count;
}