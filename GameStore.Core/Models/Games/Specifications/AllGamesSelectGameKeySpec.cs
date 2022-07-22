using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

public class AllGamesSelectGameKeySpec : Specification<Game, string>
{
    public AllGamesSelectGameKeySpec()
    {
        Query
            .Select(game => game.Key);
    }
}