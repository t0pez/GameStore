using Ardalis.Specification;
using System.Linq;

namespace GameStore.Core.Models.Games.Specifications
{
    internal class GameByPlatformTypes : Specification<Game>
    {
        public GameByPlatformTypes(PlatformType[] platformTypes)
        {
            Query.
                Where(g => platformTypes.All(x => g.PlatformTypes.Contains(x)));
        }
    }
}
