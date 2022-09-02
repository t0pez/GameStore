using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GameStore.Core.Models.Mongo.Shippers;
using GameStore.SharedKernel.Interfaces;

namespace GameStore.Core.Models.Server.Orders;

public class Order : ISafeDelete
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime ShippedDate { set; get; }

    public decimal Freight { get; set; }

    public string ShipAddress { get; set; }

    public string ShipCity { get; set; }

    public string ShipCountry { get; set; }

    public string ShipName { get; set; }

    public string ShipPostalCode { get; set; }

    public string ShipRegion { get; set; }

    public int ShipperId { get; set; }

    [NotMapped]
    public Shipper Shipper { get; set; }

    public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

    public decimal TotalSum => OrderDetails.Sum(details => details.TotalPrice) - Freight;

    public bool IsDeleted { get; set; }
}