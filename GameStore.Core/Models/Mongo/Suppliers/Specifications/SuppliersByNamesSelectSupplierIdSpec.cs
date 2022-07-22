using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;

namespace GameStore.Core.Models.Mongo.Suppliers.Specifications;

public class SuppliersByNamesSelectSupplierIdSpec : Specification<Supplier, int>
{
    public SuppliersByNamesSelectSupplierIdSpec(IEnumerable<string> names)
    {
        SupplierNames = names;
        
        Query
            .Where(supplier => names.Contains(supplier.CompanyName));

        Query
            .Select(supplier => supplier.SupplierId);
    }

    public IEnumerable<string> SupplierNames { get; set; }
}