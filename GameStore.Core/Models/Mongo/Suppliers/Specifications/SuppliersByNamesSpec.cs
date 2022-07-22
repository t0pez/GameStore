using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Mongo.Suppliers.Specifications;

public class SuppliersByNamesSpec : PagedSpec<Supplier>
{
    public SuppliersByNamesSpec(IEnumerable<string> names)
    {
        Names = names;

        Query
            .Where(supplier => names.Contains(supplier.CompanyName));
    }

    public IEnumerable<string> Names { get; set; }
}