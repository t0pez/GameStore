using Ardalis.Specification;

namespace GameStore.Core.Models.Publishers.Specifications;

public class PublisherByCompanyName : Specification<Publisher>
{
    public PublisherByCompanyName(string companyName)
    {
        Query
            .Where(publisher => publisher.Name == companyName &&
                                publisher.IsDeleted == false);
    }
}