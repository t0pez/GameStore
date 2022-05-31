using System.Linq;
using Ardalis.Specification;
using GameStore.Core.Models.Games.Specifications.Filters;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Games.Specifications;

public class GamesByFilterSpec : SafeDeleteSpec<Game>
{
    public GamesByFilterSpec(GameSearchFilter filter)
    {
        Filter = filter;

        if (string.IsNullOrEmpty(filter.Name) == false)
        {
            EnableNameFilter(filter);
        }

        if (filter.GenresIds.Any())
        {
           EnableGenresFilter();
        }
        
        if (filter.PlatformsIds.Any())
        {
            EnablePlatformsFilter();
        }

        if (filter.PublishersIds.Any())
        {
            EnablePublishersFilter();
        }

        if (Filter.MinPrice is not null)
        {
            EnableLowerPriceFilter();
        }

        if (Filter.MaxPrice is not null)
        {
            EnableHighestPriceFilter();
        }

        EnableSorting();
    }

    public GameSearchFilter Filter { get; set; }


    private void EnableNameFilter(GameSearchFilter filter)
    {
        Query
            .Where(game => game.Name.ToLower().Contains(filter.Name.ToLower()));
    }

    private void EnableGenresFilter()
    {
        Query
            .Where(game => game.Genres.Any(gg => Filter.GenresIds.Any(genreId => gg.GenreId == genreId)));
    }

    private void EnablePlatformsFilter()
    {
        Query
            .Where(game => game.Platforms.Any(gp => Filter.PlatformsIds.Any(platformId => gp.PlatformId == platformId)));
    }

    private void EnablePublishersFilter()
    {
        Query
            .Where(game => Filter.PublishersIds.Any(publisherId => game.PublisherId == publisherId));
    }

    private void EnableLowerPriceFilter()
    {
        Query
            .Where(game => game.Price >= Filter.MinPrice);
    }
    
    private void EnableHighestPriceFilter()
    {
        Query
            .Where(game => game.Price <= Filter.MaxPrice);
    }

    private void EnableSorting()
    {
        switch (Filter.OrderBy)
        {
            case GameSearchFilterOrderByState.MostPopular:
                Query
                    .OrderBy(game => game.Views);
                break;
            case GameSearchFilterOrderByState.MostCommented:
                Query
                    .OrderBy(game => game.Comments.Count(comment => comment.IsDeleted == false));
                break;
            case GameSearchFilterOrderByState.PriceAscending:
                Query
                    .OrderBy(game => game.Price);
                break;
            case GameSearchFilterOrderByState.PriceDescending:
                Query
                    .OrderByDescending(game => game.Price);
                break;
            case GameSearchFilterOrderByState.New:
                Query
                    .OrderBy(game => game.AddedToStoreAt);
                break;
            case GameSearchFilterOrderByState.Default:
            default:
                return;
        }
    }
}