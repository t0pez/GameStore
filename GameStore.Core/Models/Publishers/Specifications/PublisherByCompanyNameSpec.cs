using Ardalis.Specification;

namespace GameStore.Core.Models.Publishers.Specifications;

public sealed class PublisherByCompanyNameSpec : Specification<Publisher>
{
    public PublisherByCompanyNameSpec(string companyName)
    {
        Name = companyName;
        
        Query
            .Where(publisher => publisher.Name == companyName &&
                                publisher.IsDeleted == false);
    }

    public string Name { get; }
}