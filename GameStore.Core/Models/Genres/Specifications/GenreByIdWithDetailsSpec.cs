using System;
using System.Linq;
using Ardalis.Specification;

namespace GameStore.Core.Models.Genres.Specifications;

public sealed class GenreByIdWithDetailsSpec : GenreByIdSpec
{
    public GenreByIdWithDetailsSpec(Guid id) : base(id)
    {
        Id = id;

        Query
            .Where(genre => genre.Id == id)
            .Include(genre => genre.Parent)
            .Include(genre => genre.SubGenres.Where(subGenre => subGenre.IsDeleted == false));
    }

    public Guid Id { get; }
}