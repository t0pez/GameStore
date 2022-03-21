﻿using System;
using System.Collections.Generic;
using Ardalis.Specification;
using System.Linq;

namespace GameStore.Core.Models.Games.Specifications;

internal sealed class GamesByPlatformTypesSpec : Specification<Game>
{
    public GamesByPlatformTypesSpec(ICollection<Guid> platformTypesIds)
    {
        Query
            .Where(game => game.Platforms
                               .Select(type => type.PlatformId)
                               .All(platformTypesIds.Contains)
                           && game.IsDeleted == false);
    }
}