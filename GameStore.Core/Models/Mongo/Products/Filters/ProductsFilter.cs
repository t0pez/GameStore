using System.Collections.Generic;

namespace GameStore.Core.Models.Mongo.Products.Filters;

public class ProductFilter
{
    public string Name { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public bool IsCategoriesIdsFilterEnabled { get; set; }
    public IEnumerable<int> CategoriesIds { get; set; } = new List<int>();

    public bool IsSuppliersIdsFilterEnabled { get; set; }
    public IEnumerable<int> SuppliersIds { get; set; } = new List<int>();

    public IEnumerable<string> GameKeysToIgnore { get; set; } = new List<string>();
}