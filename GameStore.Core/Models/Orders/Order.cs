using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.SharedKernel.Interfaces;

namespace GameStore.Core.Models.Orders;

public class Order : ISafeDelete
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    
    public ICollection<OrderDetails> OrderDetails { get; set; }

    public bool IsDeleted { get; set; }

    public decimal TotalSum => OrderDetails.Sum(details => details.Price);
}