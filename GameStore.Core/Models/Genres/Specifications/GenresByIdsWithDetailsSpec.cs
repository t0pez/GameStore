using System;
using System.Collections.Generic;
using Ardalis.Specification;

namespace GameStore.Core.Models.Genres.Specifications;

public class GenresByIdsWithDetailsSpec : GenresByIdsSpec
{
    public GenresByIdsWithDetailsSpec(IEnumerable<Guid> genresIds) : base(genresIds)
    {
        Query
            .Include(genre => genre.SubGenres)
            .ThenInclude(genre => genre.SubGenres);
    }
}