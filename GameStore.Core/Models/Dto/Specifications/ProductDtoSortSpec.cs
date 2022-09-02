using System;
using System.Linq;
using System.Linq.Expressions;
using Ardalis.Specification;
using GameStore.Core.Models.Server.Games.Filters;
using SpecificationExtensions.Core.Specifications;

namespace GameStore.Core.Models.Dto.Specifications;

public class ProductDtoSortSpec : BaseSpec<ProductDto>
{
    public ProductDtoSortSpec(GameSearchFilterOrderByState orderBy, bool orderByDatabase)
    {
        OrderBy = orderBy;

        switch (orderBy)
        {
            case GameSearchFilterOrderByState.MostPopular:
                SortBy(product => product.Views, orderByDatabase);

                break;
            case GameSearchFilterOrderByState.MostCommented:
                SortBy(product => product.Comments.Count(comment => comment.IsDeleted == false),
                       orderByDatabase);

                break;
            case GameSearchFilterOrderByState.PriceAscending:
                SortBy(product => product.Price, orderByDatabase);

                break;
            case GameSearchFilterOrderByState.PriceDescending:
                SortByDescending(product => product.Price, orderByDatabase);

                break;
            case GameSearchFilterOrderByState.New:
                SortByDescending(product => product.AddedToStoreAt, orderByDatabase);

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

    private void SortBy(Expression<Func<ProductDto, object>> orderExpression, bool orderByDatabase)
    {
        if (orderByDatabase)
        {
            Query
                .OrderBy(product => product.Database)
                .ThenBy(orderExpression);
        }
        else
        {
            Query
                .OrderBy(orderExpression);
        }
    }

    private void SortByDescending(Expression<Func<ProductDto, object>> orderExpression, bool orderByDatabase)
    {
        if (orderByDatabase)
        {
            Query
                .OrderBy(product => product.Database)
                .ThenByDescending(orderExpression);
        }
        else
        {
            Query
                .OrderByDescending(orderExpression);
        }
    }
}