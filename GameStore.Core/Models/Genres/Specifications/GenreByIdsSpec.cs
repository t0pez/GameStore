using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Genres.Specifications;

public sealed class GenreByIdSpec : Specification<Genre>
{
    public GenreByIdSpec(Guid id)
    {
        Id = id;
        
        Query
            .Where(genre => genre.Id == id &&
                            genre.IsDeleted == false);
    }

    public Guid Id { get; set; }
}