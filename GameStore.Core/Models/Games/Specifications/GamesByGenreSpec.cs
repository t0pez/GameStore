using System;
using Ardalis.Specification;
using System.Linq;

namespace GameStore.Core.Models.Games.Specifications;

public sealed class GamesByGenreSpec : Specification<Game>
{
    public GamesByGenreSpec(Guid genreId)
    {
        GenreId = genreId;
        
        Query
            .Where(game => game.Genres
                               .Select(gameGenre => gameGenre.GenreId)
                               .Any(id => id == genreId)
                           && game.IsDeleted == false);
    }

    public Guid GenreId { get; }
}