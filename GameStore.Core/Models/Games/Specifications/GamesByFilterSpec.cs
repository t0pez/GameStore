using System.Linq;
using Ardalis.Specification;
using GameStore.Core.Models.Games.Specifications.Filters;
using GameStore.Core.Models.RelationalModels;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Games.Specifications;

public class GamesByFilterSpec : SafeDeleteSpec<Game>
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

        Query
            .Where(game => filter.PriceRange.Contains(game.Price));
        
        Query
            .OrderBy(filter.OrderBy);

        EnablePaging(filter);
    }

    public GameSearchFilter Filter { get; set; }

    private void EnableGenresFilter()
    {
        Query
            .Where(game => game.Genres.Join(Filter.GenresIds,
                                            gg => gg.GenreId,
                                            genreId => genreId,
                                            (genreId, _) => genreId)
                               .Count() == Filter.GenresIds.Count());
        
        Query
            .Where(game => Filter.GenresIds.Intersect(game.Genres.Select(gg => gg.GenreId))
                                 .Count() == Filter.GenresIds.Count());
        
        Query
            .Where(game => game.Genres
                               .Intersect(Filter.GenresIds.Select(
                                              genreId => new GameGenre { GameId = game.Id, GenreId = genreId }))
                               .Count() == Filter.GenresIds.Count());
        
        Query
            .Where(game => game.Genres.Select(gg => gg.GenreId)
                               .Intersect(Filter.GenresIds).Count() == Filter.GenresIds.Count());
        
        Query
            .Where(game => game.Genres
                               .Intersect(Filter.GenresIds.Select(
                                              genreId => new GameGenre { GameId = game.Id, GenreId = genreId }))
                               .SequenceEqual(
                                   Filter.GenresIds.Select(genreId => new GameGenre
                                                               { GameId = game.Id, GenreId = genreId })));
        
        Query
            .Where(game => game.Genres
                               .Intersect(Filter.GenresIds.Select(
                                              genreId => new GameGenre { GameId = game.Id, GenreId = genreId }))
                               .Count() == Filter.GenresIds.Count());
        
        Query
            .Where(game => Filter.GenresIds.Except(game.Genres.Select(gg => gg.GenreId))
                                 .Any() == false);
        
        Query
            .Where(game => Filter.GenresIds.All(genreId => game.Genres.Select(gg => gg.GenreId).Contains(genreId)));
        
        Query
            .Where(game => Filter.GenresIds.All(genreId => game.Genres.Any(gg => gg.GenreId == genreId)));

        Query
            .Where(game => game.Genres.Select(gg => gg.GenreId)
                               .SequenceEqual(Filter.GenresIds.Intersect(game.Genres.Select(gg => gg.GenreId))));
    }

    private void EnablePlatformsFilter()
    {
    }

    private void EnablePublishersFilter()
    {
        Query
            .Where(game => Filter.PublishersIds.Any(publisherId => game.PublisherId == publisherId));
    }
}

