using Ardalis.Specification;
using GameStore.Core.Models.Games;

namespace GameStore.Core.Models.PlatformTypes.Specifications;

public class PlatformTypesWithDetails : Specification<PlatformType>
{
    public PlatformTypesWithDetails()
    {
        Query
            .Where(type => type.IsDeleted == false);
    }
}