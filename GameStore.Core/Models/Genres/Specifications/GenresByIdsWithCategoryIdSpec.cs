using System;
using System.Collections.Generic;
using Ardalis.Specification;

namespace GameStore.Core.Models.Genres.Specifications;

public class GenresByIdsWithCategoryIdSpec : GenresByIdsSpec
{
    public GenresByIdsWithCategoryIdSpec(IEnumerable<Guid> genresIds) : base(genresIds)
    {
        Query
            .Where(genre => genre.CategoryId != null);
    }
}