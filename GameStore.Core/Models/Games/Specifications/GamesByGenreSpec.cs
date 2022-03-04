using Ardalis.Specification;
using System.Linq;

namespace GameStore.Core.Models.Games.Specifications
{
    public class GamesByGenreSpec : Specification<Game>
    {
        public GamesByGenreSpec(Genre genre)
        {
            Query
                .Where(g => g.Genres.Contains(genre) && g.IsDeleted == false);
        }
    }
}
