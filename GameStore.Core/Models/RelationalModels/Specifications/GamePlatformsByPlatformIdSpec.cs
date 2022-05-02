using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.RelationalModels.Specifications;

internal sealed class GamePlatformsByPlatformIdSpec : Specification<GamePlatformType>
{
    public GamePlatformsByPlatformIdSpec(Guid gameId)
    {
        GameId = gameId;

        Query
            .Where(gamePlatform => gamePlatform.GameId == gameId);
    }

    public Guid GameId { get; }
}