using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using SpecificationExtensions.Core.Specifications;

namespace GameStore.Core.Models.Mongo.Suppliers.Specifications;

public class SuppliersSpec : BaseSpec<Supplier>
{
    public SuppliersSpec ByName(string name)
    {
        Query
           .Where(supplier => supplier.CompanyName == name);

        return this;
    }

    public SuppliersSpec ByNames(IEnumerable<string> names)
    {
        Query
           .Where(supplier => names.Contains(supplier.CompanyName));

        return this;
    }
}