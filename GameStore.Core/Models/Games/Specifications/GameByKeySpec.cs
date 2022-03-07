using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications
{
    internal class GameByKeySpec : Specification<Game>, ISingleResultSpecification
    {
        public GameByKeySpec(string gameKey)
        {
            Query
                .Where(g => g.Key == gameKey 
                    && g.IsDeleted == false)
                .Include(g => g.Comments)
                .Include(g => g.Genres)
                .Include(g => g.PlatformTypes);
        }
    }
}
