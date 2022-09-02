using Ardalis.Specification;
using SpecificationExtensions.Core.Specifications;

namespace GameStore.Core.Models.Mongo.Products.Specifications;

public class ProductsSpec : BaseSpec<Product>
{
    public ProductsSpec ByGameKey(string gameKey)
    {
        Query
           .Where(product => product.GameKey == gameKey);

        return this;
    }

    public ProductsSpec WithDetails()
    {
        Query
           .Include(product => product.Category)
           .Include(product => product.Supplier);

        return this;
    }
}