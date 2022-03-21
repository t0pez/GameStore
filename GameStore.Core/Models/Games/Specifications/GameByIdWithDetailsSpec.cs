using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

internal sealed class GameByIdWithDetailsSpec : Specification<Game>, ISingleResultSpecification
{
    public GameByIdWithDetailsSpec(Guid id)
    {
        Query
            .Where(game => game.Id == id
                           && game.IsDeleted == false)
            .Include(game => game.Genres)
            .Include(game => game.Platforms);
    }
}