using System;
using System.Collections.Generic;
using GameStore.Core.Models.Server.Orders;

namespace GameStore.Web.Models.Order;

public class OrderUpdateRequestModel
{
    public Guid Id { get; set; }

    public DateTime OrderDate { get; set; }

    public OrderStatus Status { get; set; }

    public string ShipAddress { get; set; }

    public string ShipCity { get; set; }

    public string ShipCountry { get; set; }

    public string ShipName { get; set; }

    public string ShipPostalCode { get; set; }

    public string ShipRegion { get; set; }

    public List<OrderDetails> OrderDetails { get; set; }
}