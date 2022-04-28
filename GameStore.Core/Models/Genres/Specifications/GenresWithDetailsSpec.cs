using System.Linq;
using Ardalis.Specification;

namespace GameStore.Core.Models.Genres.Specifications;

public sealed class GenresWithDetailsSpec : Specification<Genre>
{
    public GenresWithDetailsSpec()
    {
        Query
            .Where(genre => genre.IsDeleted == false)
            .Include(genre => genre.Games)
            .Include(genre => genre.SubGenres.Where(subGenre => subGenre.IsDeleted == false));
    }
}