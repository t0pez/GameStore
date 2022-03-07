using Ardalis.Specification;
using System.Linq;

namespace GameStore.Core.Models.Games.Specifications
{
    internal class GamesByPlatformTypesSpec : Specification<Game>
    {
        public GamesByPlatformTypesSpec(PlatformType[] platformTypes)
        {
            Query
                .Where(g => g.PlatformTypes.All(x => platformTypes.Contains(x))
                    && g.IsDeleted == false);
        }
    }
}
