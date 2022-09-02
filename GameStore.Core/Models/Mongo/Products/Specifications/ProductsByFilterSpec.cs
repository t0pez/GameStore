using System.Linq;
using Ardalis.Specification;
using GameStore.Core.Models.Mongo.Products.Filters;
using SpecificationExtensions.Core.Specifications;

namespace GameStore.Core.Models.Mongo.Products.Specifications;

public class ProductsByFilterSpec : BaseSpec<Product>
{
    public ProductsByFilterSpec(ProductFilter filter)
    {
        Filter = filter;

        if (string.IsNullOrEmpty(filter.Name) == false)
        {
            Query
                .Where(product => product.ProductName.ToLower().Contains(filter.Name));
        }

        if (filter.MinPrice.HasValue)
        {
            Query
                .Where(product => product.UnitPrice >= filter.MinPrice);
        }

        if (filter.MaxPrice.HasValue)
        {
            Query
                .Where(product => product.UnitPrice <= filter.MaxPrice);
        }

        if (filter.IsCategoriesIdsFilterEnabled)
        {
            Query
                .Where(product => filter.CategoriesIds.Contains(product.CategoryId));
        }

        if (filter.IsSuppliersIdsFilterEnabled)
        {
            Query
                .Where(product => filter.SuppliersIds.Contains(product.CategoryId));
        }
    }

    public ProductFilter Filter { get; set; }
}