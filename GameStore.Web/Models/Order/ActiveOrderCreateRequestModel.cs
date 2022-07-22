using System;

namespace GameStore.Web.Models.Order;

public class ActiveOrderCreateRequestModel
{
    public Guid OrderId { get; set; }
    
    public string ShipAddress { get; set; }
    public string ShipCity { get; set; }
    public string ShipCountry { get; set; }
    public string ShipName { get; set; }
    public string ShipPostalCode { get; set; }
    public string ShipRegion { get; set; }
    public int ShipperId { get; set; }
}