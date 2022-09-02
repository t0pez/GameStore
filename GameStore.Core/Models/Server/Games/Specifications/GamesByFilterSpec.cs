using System;
using System.Linq;
using Ardalis.Specification;
using GameStore.Core.Models.Server.Games.Filters;

namespace GameStore.Core.Models.Server.Games.Specifications;

public class GamesByFilterSpec : GamesSpec
{
    public GamesByFilterSpec(GameSearchFilter filter)
    {
        Filter = filter;

        if (string.IsNullOrEmpty(filter.Name) == false)
        {
            Query
               .Where(game => game.Name.ToLower().Contains(filter.Name.ToLower()));
        }

        if (filter.GenresIds.Any())
        {
            Query
               .Where(game => game.Genres.Any(gg => Filter.GenresIds.Any(genreId => gg.GenreId == genreId)));
        }

        if (filter.PlatformsIds.Any())
        {
            Query
               .Where(game => game.Platforms.Any(
                          gp => Filter.PlatformsIds.Any(platformId => gp.PlatformId == platformId)));
        }

        if (filter.PublishersNames.Any())
        {
            Query
               .Where(game => Filter.PublishersNames.Any(name => game.PublisherName == name));
        }

        if (Filter.MinPrice is not null)
        {
            Query
               .Where(game => game.Price >= Filter.MinPrice);
        }

        if (Filter.MaxPrice is not null)
        {
            Query
               .Where(game => game.Price <= Filter.MaxPrice);
        }

        if (Filter.PublishedAtState != GameSearchFilterPublishedAtState.Default)
        {
            EnablePublishedAtFilter();
        }
    }

    public GameSearchFilter Filter { get; set; }

    private void EnablePublishedAtFilter()
    {
        switch (Filter.PublishedAtState)
        {
            case GameSearchFilterPublishedAtState.OneWeek:
                Query
                   .Where(game => game.PublishedAt >= DateTime.UtcNow.AddDays(-7));

                break;
            case GameSearchFilterPublishedAtState.OneMonth:
                Query
                   .Where(game => game.PublishedAt >= DateTime.UtcNow.AddMonths(-1));

                break;
            case GameSearchFilterPublishedAtState.OneYear:
                Query
                   .Where(game => game.PublishedAt >= DateTime.UtcNow.AddYears(-1));

                break;
            case GameSearchFilterPublishedAtState.TwoYears:
                Query
                   .Where(game => game.PublishedAt >= DateTime.UtcNow.AddYears(-2));

                break;
            case GameSearchFilterPublishedAtState.ThreeYears:
                Query
                   .Where(game => game.PublishedAt >= DateTime.UtcNow.AddYears(-3));

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}