using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStore.Web.ViewModels.Order;

public class OrderViewModel
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public IEnumerable<OrderDetailsViewModel> OrderDetails { get; set; } = new List<OrderDetailsViewModel>();

    public decimal TotalSum => OrderDetails.Sum(model => model.Price);
}