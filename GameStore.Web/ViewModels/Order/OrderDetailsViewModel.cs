using System;
using GameStore.Web.ViewModels.Games;

namespace GameStore.Web.ViewModels.Order;

public class OrderDetailsViewModel
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalSum { get; set; }
    
    public GameInOrderDetailsViewModel Game { get; set; }
}