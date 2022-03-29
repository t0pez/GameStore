using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

public sealed class GameByKeySpec : Specification<Game>, ISingleResultSpecification
{
    public GameByKeySpec(string gameKey)
    {
        Key = gameKey;
        
        Query
            .Where(game => game.Key == gameKey
                           && game.IsDeleted == false);
    }

    public string Key { get; }
}