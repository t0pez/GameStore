using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Genres.Specifications;

public class GenreByIdSpec : SafeDeleteSpec<Genre>
{
    public GenreByIdSpec(Guid id)
    {
        Id = id;

        Query
            .Where(genre => genre.Id == id);
    }

    public Guid Id { get; }
}