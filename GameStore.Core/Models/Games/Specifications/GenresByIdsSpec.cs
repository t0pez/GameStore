using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

internal sealed class GenresByIdsSpec : Specification<Genre>
{
    public GenresByIdsSpec(ICollection<Guid> ids)
    {
        Query
            .Where(genre => ids.Any(id => genre.Id == id));
    }
}