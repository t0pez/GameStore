using System;
using System.Linq;
using System.Linq.Expressions;
using Ardalis.Specification;
using GameStore.Core.Models.Games.Specifications.Filters;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Dto.Specifications;

public class ProductDtoSortSpec : PagedSpec<ProductDto>
{
    public ProductDtoSortSpec(GameSearchFilterOrderByState orderBy, bool thenByDatabase)
    {
        OrderBy = orderBy;

        switch (orderBy)
        {
            case GameSearchFilterOrderByState.MostPopular:
                SortBy(product => product.Views, thenByDatabase);
                break;
            case GameSearchFilterOrderByState.MostCommented:
                SortBy(product => product.Comments.Count(comment => comment.IsDeleted == false),
                       thenByDatabase);
                break;
            case GameSearchFilterOrderByState.PriceAscending:
                SortBy(product => product.Price, thenByDatabase);
                break;
            case GameSearchFilterOrderByState.PriceDescending:
                Query
                    .OrderBy(product => product.Price)
                    .ThenBy(product => product.Database, thenByDatabase);
                break;
            case GameSearchFilterOrderByState.New:
                SortBy(product => product.AddedToStoreAt, thenByDatabase);
                break;
            case GameSearchFilterOrderByState.Default:
                Query
                    .OrderBy(product => product.Database);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(orderBy), orderBy, null);
        }
    }

    public GameSearchFilterOrderByState OrderBy { get; set; }

    private void SortBy(Expression<Func<ProductDto, object>> sortExpression, bool thenByDatabase)
    {
        Query
            .OrderBy(sortExpression)
            .ThenBy(product => product.Database, thenByDatabase);
    }
}