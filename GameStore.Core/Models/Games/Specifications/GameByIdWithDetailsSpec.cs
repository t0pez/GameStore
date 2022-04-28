using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

public sealed class GameByIdWithDetailsSpec : Specification<Game>, ISingleResultSpecification
{
    public GameByIdWithDetailsSpec(Guid id)
    {
        Id = id;
        
        Query
            .Where(game => game.Id == id
                           && game.IsDeleted == false)
            .Include(game => game.Genres)
            .Include(game => game.Platforms);
    }

    public Guid Id { get; }
}