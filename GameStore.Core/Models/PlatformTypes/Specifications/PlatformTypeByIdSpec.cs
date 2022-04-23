using System;
using Ardalis.Specification;
using GameStore.Core.Models.Games;

namespace GameStore.Core.Models.PlatformTypes.Specifications;

public sealed class PlatformTypeByIdSpec : Specification<PlatformType>
{
    public PlatformTypeByIdSpec(Guid id)
    {
        Query
            .Where(platformType => platformType.Id == id
                                   && platformType.IsDeleted == false);
    }
}