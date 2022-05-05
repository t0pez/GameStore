using System;
using System.Collections.Generic;
using GameStore.Core.Models.Orders;

namespace GameStore.Core.Models.ServiceModels.Orders;

public class OrderUpdateModel
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public ICollection<OrderDetails> OrderDetails { get; set; }
}