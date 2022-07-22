using System.Linq;
using Ardalis.Specification;
using GameStore.Core.Models.Mongo.Products.Filters;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Mongo.Products.Specifications;

public class ProductsByFilterSpec : PagedSpec<Product>
{
    public ProductsByFilterSpec(ProductFilter filter)
    {
        Filter = filter;
        
        if (filter.GameKeysToIgnore.Any())
        {
            Query
                .Where(product => product.GameKey != null);

            foreach (var gameKey in filter.GameKeysToIgnore)
            {
                Query
                    .Where(product => product.GameKey != gameKey);
            }
        }
        
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

        if (filter.CategoriesIds.Any())
        {
            Query
                .Where(product => filter.CategoriesIds.Contains(product.CategoryId));
        }
        
        if (filter.SuppliersIds.Any())
        {
            Query
                .Where(product => filter.SuppliersIds.Contains(product.CategoryId));
        }
    }

    public ProductFilter Filter { get; set; }
}