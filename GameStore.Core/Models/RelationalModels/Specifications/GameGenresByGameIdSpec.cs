using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.RelationalModels.Specifications;

internal sealed class GameGenresByGameIdSpec : Specification<GameGenre>
{
    public GameGenresByGameIdSpec(Guid gameId)
    {
        GameId = gameId;
        
        Query
            .Where(gameGenre => gameGenre.GameId == gameId);
    }

    public Guid GameId { get; }
}