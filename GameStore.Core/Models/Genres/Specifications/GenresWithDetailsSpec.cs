using System.Linq;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Genres.Specifications;

public sealed class GenresWithDetailsSpec : MultipleResultSafeDeleteSpec<Genre>
{
    public GenresWithDetailsSpec()
    {
        Query
            .Include(genre => genre.Games)
            .Include(genre => genre.SubGenres.Where(subGenre => subGenre.IsDeleted == false));
    }
}