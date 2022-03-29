using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

public sealed class GenreByIdSpec : Specification<Genre>
{
    public GenreByIdSpec(Guid id)
    {
        Query
            .Where(genre => genre.Id == id
                            && genre.IsDeleted == false);
    }
}