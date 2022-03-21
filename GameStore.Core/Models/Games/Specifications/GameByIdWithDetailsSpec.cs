using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

public sealed class GameByIdWithDetailsSpec : Specification<Game>, ISingleResultSpecification
{
    public GameByIdWithDetailsSpec(Guid id)
    {
        Query
            .Include(game => game.Genres)
            .Include(game => game.Platforms)
            .Where(game => game.Id == id
                   && game.IsDeleted == false);
    }
}