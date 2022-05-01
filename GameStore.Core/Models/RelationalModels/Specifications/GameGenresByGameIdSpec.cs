using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.RelationalModels.Specifications;

internal sealed class GameGenresByGameIdSpec : MultipleResultDomainSpec<GameGenre>
{
    public GameGenresByGameIdSpec(Guid gameId)
    {
        GameId = gameId;

        Query
            .Where(gameGenre => gameGenre.GameId == gameId);
    }

    public Guid GameId { get; }
}