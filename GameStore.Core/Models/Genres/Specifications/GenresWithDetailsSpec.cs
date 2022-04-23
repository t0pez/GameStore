using Ardalis.Specification;
using GameStore.Core.Models.Games;

namespace GameStore.Core.Models.Genres.Specifications;

public class GenresWithDetailsSpec : Specification<Genre>
{
    public GenresWithDetailsSpec()
    {
        Query
            .Where(genre => genre.IsDeleted == false)
            .Include(genre => genre.Games); // TODO: set selector for children IsDeleted state
    }
}