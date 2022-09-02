using Ardalis.Specification;
using SpecificationExtensions.Core.Specifications;

namespace GameStore.Core.Models.Mongo.Shippers.Specifications;

public class ShippersSpec : BaseSpec<Shipper>
{
    public ShippersSpec ByName(string name)
    {
        Query
           .Where(shipper => shipper.CompanyName == name);

        return this;
    }
}