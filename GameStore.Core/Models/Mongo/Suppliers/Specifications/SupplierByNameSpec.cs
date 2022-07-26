using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Mongo.Suppliers.Specifications;

public class SupplierByNameSpec : PagedSpec<Supplier>
{
    public SupplierByNameSpec(string name)
    {
        Name = name;

        Query
            .Where(supplier => supplier.CompanyName == name);
    }

    public string Name { get; set; }
}