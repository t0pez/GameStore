using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

public sealed class GameByKeyWithDetailsSpec : GameByKeySpec
{
    public GameByKeyWithDetailsSpec(string gameKey) : base(gameKey)
    {
        Query
            .Include(g => g.Comments)
            .Include(g => g.Genres)
            .ThenInclude(gg => gg.Genre)
            .Include(g => g.Platforms)
            .ThenInclude(gp => gp.Platform);
    }
}