using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Server.Games.Specifications;

public class GamesSpec : SafeDeleteSpec<Game>
{
    public GamesSpec ById(Guid id)
    {
        Query
           .Where(game => game.Id == id);

        return this;
    }

    public GamesSpec ByKey(string gameKey)
    {
        Query
           .Where(game => game.Key == gameKey);

        return this;
    }

    public GamesSpec ByPublisherName(string publisherName)
    {
        Query
           .Where(game => game.PublisherName == publisherName);

        return this;
    }

    public GamesSpec WithDetails()
    {
        Query
           .Include(g => g.Comments)
           .Include(g => g.Genres)
           .ThenInclude(gameGenre => gameGenre.Genre)
           .ThenInclude(genre => genre.SubGenres)
           .Include(g => g.Platforms)
           .ThenInclude(gamePlatform => gamePlatform.Platform);

        return this;
    }
}