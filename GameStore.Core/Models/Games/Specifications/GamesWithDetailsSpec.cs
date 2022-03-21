using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

internal sealed class GamesWithDetailsSpec : Specification<Game>
{
    public GamesWithDetailsSpec()
    {
        Query
            .Where(g => g.IsDeleted == false)
            .Include(g => g.Comments)
            .Include(g => g.Genres)
            .ThenInclude(gameGenre => gameGenre.Genre)
            .Include(g => g.Platforms)
            .ThenInclude(gamePlatform => gamePlatform.Platform);
    }
}