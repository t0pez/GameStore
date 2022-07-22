using Ardalis.Specification;

namespace GameStore.Core.Models.Mongo.Products.Specifications;

public class ProductsByGameKeySpec : Specification<Product>
{
    public ProductsByGameKeySpec(string gameKey)
    {
        GameKey = gameKey;

        Query
            .Where(product => product.GameKey == gameKey);
    }

    public string GameKey { get; set; }
}