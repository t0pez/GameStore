using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Games.Specifications;

public class GamesByPlatformTypesSpec : SafeDeleteSpec<Game>
{
    public GamesByPlatformTypesSpec(ICollection<Guid> platformTypesIds)
    {
        PlatformTypesIds = platformTypesIds;

        Query
            .Where(game => game.Platforms
                               .Select(type => type.PlatformId)
                               .All(platformTypesIds.Contains));
    }

    public ICollection<Guid> PlatformTypesIds { get; }
}