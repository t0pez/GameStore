using Ardalis.Specification;

namespace GameStore.Core.Models.Mongo.Products.Specifications;

public class ProductByGameKeyWithDetailsSpec : ProductByGameKeySpec
{
    public ProductByGameKeyWithDetailsSpec(string gameKey) : base(gameKey)
    {
        Query
            .Include(product => product.Category)
            .Include(product => product.Supplier);
    }
}