using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Genres.Specifications;

public class GenreByCategoryIdSpec : SafeDeleteSpec<Genre>
{
    public GenreByCategoryIdSpec(int categoryId)
    {
        CategoryId = categoryId;

        Query
            .Where(genre => genre.CategoryId == categoryId);
    }

    public int CategoryId { get; set; }
}