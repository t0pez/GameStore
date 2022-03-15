using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

internal class GamesWithDetailsSpec : Specification<Game>
{
    public GamesWithDetailsSpec()
    {
        Query
            .Where(g => g.IsDeleted == false)
            .Include(g => g.Comments)
            .Include(g => g.Genres)
            .Include(g => g.Platforms);
    }
}