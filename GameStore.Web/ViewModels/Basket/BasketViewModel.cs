using System.Collections.Generic;

namespace GameStore.Web.ViewModels.Basket;

public class BasketViewModel
{
    public ICollection<BasketItemViewModel> Items { get; set; } = new List<BasketItemViewModel>();
}