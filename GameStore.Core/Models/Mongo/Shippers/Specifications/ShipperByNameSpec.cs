using SpecificationExtensions.Core.Specifications;

namespace GameStore.Core.Models.Mongo.Shippers.Specifications;

public class ShipperByNameSpec : BaseSpec<Shipper>
{
    public ShipperByNameSpec(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}