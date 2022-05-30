using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Genres.Specifications;

public class GenresByIdsWithDetails : SafeDeleteSpec<Genre>
{
    public GenresByIdsWithDetails(IEnumerable<Guid> genresIds)
    {
        GenresIds = genresIds;

        Query
            .Where(genre => genresIds.Contains(genre.Id))
            .Include(genre => genre.SubGenres)
            .ThenInclude(genre => genre.SubGenres);
    }

    public IEnumerable<Guid> GenresIds { get; set; }
}