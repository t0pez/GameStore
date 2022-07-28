using Ardalis.Specification;
using SpecificationExtensions.Core.Specifications;

namespace GameStore.Core.Models.Mongo.Suppliers.Specifications;

public class SupplierByNameSpec : BaseSpec<Supplier>
{
    public SupplierByNameSpec(string name)
    {
        Name = name;

        Query
            .Where(supplier => supplier.CompanyName == name);
    }

    public string Name { get; set; }
}