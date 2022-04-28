using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

public sealed class GamesListSpec : Specification<Game>
{
    public GamesListSpec()
    {
        Query
            .Where(game => game.IsDeleted == false);
    }
}