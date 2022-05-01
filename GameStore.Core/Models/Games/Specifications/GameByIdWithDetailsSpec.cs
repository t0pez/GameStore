using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

public sealed class GameByIdWithDetailsSpec : GameByIdSpec
{
    public GameByIdWithDetailsSpec(Guid id) : base(id)
    {
        Query
            .Include(game => game.Genres)
            .Include(game => game.Platforms);
    }
}