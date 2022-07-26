using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Mongo.Products.Specifications;

public class ProductByGameKeySpec : PagedSpec<Product>
{
    public ProductByGameKeySpec(string gameKey)
    {
        GameKey = gameKey;

        Query
            .Where(product => product.GameKey == GameKey);
    }

    public string GameKey { get; set; }
}