using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

public sealed class GameByKeyWithDetailsSpec : GameByKeySpec
{
    public GameByKeyWithDetailsSpec(string gameKey) : base(gameKey)
    {
        Query
            .Include(g => g.Comments)
            .Include(g => g.Genres)
            .Include(g => g.Platforms)
            .Include(game => game.Publisher);
    }
}