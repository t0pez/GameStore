using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.Core.Models.Mongo.Shippers;
using GameStore.Core.Models.Server.Orders;

namespace GameStore.Core.Models.Dto;

public class OrderDto
{
    public string CustomerId { get; set; }

    public OrderStatus? Status { get; set; }

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

    public Shipper Shipper { get; set; }

    public ICollection<OrderDetailsDto> OrderDetails { get; set; } = new List<OrderDetailsDto>();

    public bool IsDeleted { get; set; }

    public decimal TotalSum => OrderDetails.Sum(dto => dto.TotalPrice);
}