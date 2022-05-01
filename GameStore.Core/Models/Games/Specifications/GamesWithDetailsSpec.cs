using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Games.Specifications;

public sealed class GamesWithDetailsSpec : MultipleResultSafeDeleteSpec<Game>
{
    public GamesWithDetailsSpec()
    {
        Query
            .Include(g => g.Comments)
            .Include(g => g.Genres)
            .ThenInclude(gameGenre => gameGenre.Genre)
            .ThenInclude(genre => genre.SubGenres)
            .Include(g => g.Platforms)
            .ThenInclude(gamePlatform => gamePlatform.Platform);
    }
}