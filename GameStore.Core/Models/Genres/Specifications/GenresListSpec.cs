using Ardalis.Specification;

namespace GameStore.Core.Models.Genres.Specifications;

public sealed class GenresListSpec : Specification<Genre>
{
    public GenresListSpec()
    {
        Query
            .Where(genre => genre.IsDeleted == false);
    }
}