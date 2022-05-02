using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Games.Specifications;

public class GameByIdSpec : SafeDeleteSpec<Game>
{
    public GameByIdSpec(Guid id)
    {
        Id = id;

        Query
            .Where(game => game.Id == id);
    }

    public Guid Id { get; }
}