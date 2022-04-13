﻿using GameStore.Web.ViewModels.Games;

namespace GameStore.Web.ViewModels.Basket;

public class BasketItemViewModel
{
    public GameInBasketViewModel Game { get; set; }
    public int Quantity { get; set; }
}