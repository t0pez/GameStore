using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

public sealed class GamesWithDetailsSpec : Specification<Game>
{
    public GamesWithDetailsSpec()
    {
        Query
            .Where(g => g.IsDeleted == false)
            .Include(g => g.Comments)
            .Include(g => g.Genres)
            .ThenInclude(gameGenre => gameGenre.Genre)
            .ThenInclude(genre => genre.SubGenres)
            .Include(g => g.Platforms)
            .ThenInclude(gamePlatform => gamePlatform.Platform);
    }
}