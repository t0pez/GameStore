using System;
using Ardalis.Specification;
using GameStore.Core.Models.Games;

namespace GameStore.Core.Models.Genres.Specifications;

public sealed class GenreByIdSpec : Specification<Genre>
{
    public GenreByIdSpec(Guid id)
    {
        Query
            .Where(genre => genre.Id == id
                            && genre.IsDeleted == false);
    }
}