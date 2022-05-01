using System;
using Ardalis.Specification;
using System.Linq;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Games.Specifications;

public class GamesByGenreSpec : MultipleResultSafeDeleteSpec<Game>
{
    public GamesByGenreSpec(Guid genreId)
    {
        GenreId = genreId;

        Query
            .Where(game => game.Genres
                               .Select(gameGenre => gameGenre.GenreId)
                               .Any(id => id == genreId));
    }

    public Guid GenreId { get; }
}