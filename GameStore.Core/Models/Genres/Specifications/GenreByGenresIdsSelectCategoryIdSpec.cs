using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;

namespace GameStore.Core.Models.Genres.Specifications;

public class GenreByGenresIdsSelectCategoryIdSpec : Specification<Genre, int>
{
    public GenreByGenresIdsSelectCategoryIdSpec(IEnumerable<Guid> genreIds)
    {
        GenreIds = genreIds;

        Query
            .Where(genre => genre.CategoryId != null)
            .Where(genre => GenreIds.Contains(genre.Id));

        Query
            .Select(genre => genre.CategoryId.Value);
    }

    public IEnumerable<Guid> GenreIds { get; set; }
}