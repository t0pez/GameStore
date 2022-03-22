using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.RelationalModels.Specifications;

internal sealed class GameGenresByGameId : Specification<GameGenre>
{
    public GameGenresByGameId(Guid gameId)
    {
        Query
            .Where(gameGenre => gameGenre.GameId == gameId);
    }
}