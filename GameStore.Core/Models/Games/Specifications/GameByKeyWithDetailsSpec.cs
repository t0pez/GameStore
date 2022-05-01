using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

public sealed class GameByKeyWithDetailsSpec : GameByKeySpec
{
    public GameByKeyWithDetailsSpec(string gameKey) : base(gameKey)
    {
        Key = gameKey;

        Query
            .Where(g => g.Key == gameKey)
            .Include(g => g.Comments)
            .Include(g => g.Genres)
            .ThenInclude(gameGenre => gameGenre.Genre)
            .Include(g => g.Platforms)
            .ThenInclude(gamePlatform => gamePlatform.Platform)
            .Include(game => game.Publisher);
    }

    public string Key { get; }
}