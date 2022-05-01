using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Publishers.Specifications;

public class PublisherByCompanyNameSpec : SingleResultSafeDeleteSpec<Publisher>
{
    public PublisherByCompanyNameSpec(string companyName)
    {
        Name = companyName;

        Query
            .Where(publisher => publisher.Name == companyName);
    }

    public string Name { get; }
}