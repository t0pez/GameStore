using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Mongo.Shippers.Specifications;

public class ShipperByNameSpec : PagedSpec<Shipper>
{
    public ShipperByNameSpec(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}