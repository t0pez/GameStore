using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Games.Specifications;

public class GameByKeySpec : SingleResultSafeDeleteSpec<Game>
{
    public GameByKeySpec(string gameKey)
    {
        Key = gameKey;

        Query
            .Where(game => game.Key == gameKey);
    }

    public string Key { get; }
}