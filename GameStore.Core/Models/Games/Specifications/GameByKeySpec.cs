using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

internal sealed class GameByKeySpec : Specification<Game>, ISingleResultSpecification
{
    public GameByKeySpec(string gameKey)
    {
        Query
            .Where(game => game.Key == gameKey
                           && game.IsDeleted == false);
    }
}