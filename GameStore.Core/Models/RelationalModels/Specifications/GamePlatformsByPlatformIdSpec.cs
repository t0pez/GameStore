using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.RelationalModels.Specifications;

internal sealed class GamePlatformsByPlatformIdSpec : MultipleResultDomainSpec<GamePlatformType>
{
    public GamePlatformsByPlatformIdSpec(Guid gameId)
    {
        GameId = gameId;

        Query
            .Where(gamePlatform => gamePlatform.GameId == gameId);
    }

    public Guid GameId { get; }
}