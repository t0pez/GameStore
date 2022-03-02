using Ardalis.Specification;
using System.Linq;

namespace GameStore.Core.Models.Games.Specifications
{
    public class GameByGenreSpec : Specification<Game>
    {
        public GameByGenreSpec(Genre genre)
        {
            Query.
                Where(g => g.Genres.Contains(genre));
        }
    }
}
