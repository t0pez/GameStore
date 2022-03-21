using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

public class PlatformTypeByIdSpec : Specification<PlatformType>
{
    public PlatformTypeByIdSpec(Guid id)
    {
        Query
            .Where(platformType => platformType.Id == id
                                   && platformType.IsDeleted == false);
    }
}