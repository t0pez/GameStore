using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

public sealed class GameByIdSpec : Specification<Game>
{
    public GameByIdSpec(Guid id)
    {
        Id = id; 
        
        Query
            .Where(game => game.Id == id
                           && game.IsDeleted == false);
    }

    public Guid Id { get; }
}