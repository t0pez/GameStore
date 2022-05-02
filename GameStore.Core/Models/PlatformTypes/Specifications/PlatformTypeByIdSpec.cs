using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.PlatformTypes.Specifications;

public class PlatformTypeByIdSpec : SafeDeleteSpec<PlatformType>
{
    public PlatformTypeByIdSpec(Guid id)
    {
        Id = id;

        Query
            .Where(platformType => platformType.Id == id);
    }

    public Guid Id { get; }
}