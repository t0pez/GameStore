using System;
using System.Linq;
using Ardalis.Specification;

namespace GameStore.Core.Models.Genres.Specifications;

public sealed class GenreByIdWithDetailsSpec : Specification<Genre>, ISingleResultSpecification
{
    public GenreByIdWithDetailsSpec(Guid id)
    {
        Query
            .Where(genre => genre.Id == id &&
                            genre.IsDeleted == false)
            .Include(genre => genre.Parent)
            .Include(genre => genre.SubGenres.Where(subGenre => subGenre.IsDeleted == false));
    }
}