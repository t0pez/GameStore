using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Genres.Specifications;

public class GenresByIdsSpec : SafeDeleteSpec<Genre>
{
    public GenresByIdsSpec(IEnumerable<Guid> genresIds)
    {
        GenresIds = genresIds;

        Query
            .Where(genre => genresIds.Contains(genre.Id));
    }

    public IEnumerable<Guid> GenresIds { get; set; }
}