using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.RelationalModels.Specifications;

public class GamePlatformsByPlatformId : Specification<GamePlatformType>
{
    public GamePlatformsByPlatformId(Guid gameId)
    {
        Query
            .Where(gamePlatform => gamePlatform.GameId == gameId);
    }
}