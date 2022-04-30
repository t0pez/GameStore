using Ardalis.Specification;
using GameStore.Core.Models.Games;

namespace GameStore.Core.Models.PlatformTypes.Specifications;

public sealed class PlatformTypesWithDetailsSpec : Specification<PlatformType>
{
    public PlatformTypesWithDetailsSpec()
    {
        Query
            .Where(type => type.IsDeleted == false);
    }
}