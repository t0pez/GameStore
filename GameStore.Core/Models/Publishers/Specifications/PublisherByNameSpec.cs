using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Publishers.Specifications;

public class PublisherByNameSpec : SafeDeleteSpec<Publisher>
{
    public PublisherByNameSpec(string companyName)
    {
        Name = companyName;

        Query
            .Where(publisher => publisher.Name == companyName);
    }

    public string Name { get; }
}