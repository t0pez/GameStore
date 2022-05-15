using System;

namespace GameStore.Web.ViewModels.Order;

public class OrderListViewModel
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Price { get; set; }
    public int ItemsCount { get; set; }
}