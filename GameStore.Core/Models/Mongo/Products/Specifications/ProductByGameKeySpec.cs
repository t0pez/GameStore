using Ardalis.Specification;
using SpecificationExtensions.Core.Specifications;

namespace GameStore.Core.Models.Mongo.Products.Specifications;

public class ProductByGameKeySpec : BaseSpec<Product>
{
    public ProductByGameKeySpec(string gameKey)
    {
        GameKey = gameKey;

        Query
            .Where(product => product.GameKey == GameKey);
    }

    public string GameKey { get; set; }
}