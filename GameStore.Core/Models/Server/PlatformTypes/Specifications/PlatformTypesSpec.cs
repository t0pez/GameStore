using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Server.PlatformTypes.Specifications;

public class PlatformTypesSpec : SafeDeleteSpec<PlatformType>
{
    public PlatformTypesSpec ById(Guid id)
    {
        Query
           .Where(type => type.Id == id);

        return this;
    }
}