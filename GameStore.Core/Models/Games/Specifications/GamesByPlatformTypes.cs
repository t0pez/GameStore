using Ardalis.Specification;
using System.Linq;

namespace GameStore.Core.Models.Games.Specifications
{
    internal class GamesByPlatformTypes : Specification<Game>
    {
        public GamesByPlatformTypes(PlatformType[] platformTypes)
        {
            Query
                .Where(g => platformTypes.All(x => g.PlatformTypes.Contains(x)) && g.IsDeleted == false);
        }
    }
}
