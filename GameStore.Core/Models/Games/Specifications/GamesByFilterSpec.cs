using System.Linq;
using Ardalis.Specification;
using GameStore.Core.Models.Games.Specifications.Filters;
using GameStore.SharedKernel;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Games.Specifications;

public class GamesByFilterSpec : SafeDeleteSpec<Game>
{
    public GamesByFilterSpec(GameSearchFilter filter)
    {
        Filter = filter;

        if (string.IsNullOrEmpty(filter.Name))
        {
            Query
                .Where(game => game.Name.ToLower().Contains(filter.Name.ToLower()));
        }

        if (filter.GenresIds.Any())
        {
            Query
                .Where(game => filter.GenresIds.All(id => game.Genres.Select(gg => gg.GenreId).Contains(id)));
        }

        if (filter.PlatformsIds.Any())
        {
            Query
                .Where(game => filter.PlatformsIds.All(id => game.Platforms.Select(gp => gp.PlatformId).Contains(id)));
        }

        if (filter.PublisherId is not null)
        {
            Query
                .Where(game => game.PublisherId == filter.PublisherId);
        }

        if (filter.PriceRange is not null)
        {
            Query
                .Where(game => game.Price.IsInRange(filter.PriceRange));
        }

        EnablePaging(filter);

        Query
            .OrderBy(filter.OrderBy);
    }

    public GameSearchFilter Filter { get; set; }
}