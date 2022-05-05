using System.Collections.Generic;

namespace GameStore.Web.ViewModels.Baskets;

public class BasketViewModel
{
    public ICollection<BasketItemViewModel> Items { get; set; } = new List<BasketItemViewModel>();
}