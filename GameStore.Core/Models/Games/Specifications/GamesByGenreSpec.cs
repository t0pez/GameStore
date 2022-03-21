using System;
using Ardalis.Specification;
using System.Linq;

namespace GameStore.Core.Models.Games.Specifications;

public class GamesByGenreSpec : Specification<Game>
{
    public GamesByGenreSpec(Guid genreId)
    {
        Query
            .Where(g => g.Genres.Select(gameGenre => gameGenre.GenreId)
                         .Any(id => id == genreId)
                        && g.IsDeleted == false);
    }
}