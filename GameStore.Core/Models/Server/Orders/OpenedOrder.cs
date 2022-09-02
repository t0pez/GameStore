using System;

namespace GameStore.Core.Models.Server.Orders;

public class OpenedOrder
{
    public Guid OrderId { get; set; }

    public DateTime TimeOutDate { get; set; }
}