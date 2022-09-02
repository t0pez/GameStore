using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStore.Web.ViewModels.Order;

public class OrderListViewModel
{
    public Guid Id { get; set; }

    public string CustomerId { get; set; }

    public decimal Price { get; set; }

    public decimal TotalSum { get; set; }

    public IEnumerable<OrderDetailsViewModel> OrderDetails { get; set; }

    public int ItemsCount => OrderDetails.Count();
}