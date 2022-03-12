using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

public class PlatformTypesByIdsSpec : Specification<PlatformType>
{
    public PlatformTypesByIdsSpec(ICollection<Guid> ids)
    {
        Query
            .Where(platformType => ids.Any(id => platformType.Id == id));
    }
}