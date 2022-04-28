using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.PlatformTypes.Specifications;

public sealed class PlatformTypeByIdSpec : Specification<PlatformType>
{
    public PlatformTypeByIdSpec(Guid id)
    {
        Id = id;
        
        Query
            .Where(platformType => platformType.Id == id
                                   && platformType.IsDeleted == false);
    }

    public Guid Id { get; }
}