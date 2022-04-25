using Ardalis.Specification;

namespace GameStore.Core.Models.Publishers.Specifications;

public class PublishersWithDetailsSpec : Specification<Publisher>
{
    public PublishersWithDetailsSpec()
    {
        Query
            .Where(publisher => publisher.IsDeleted == false);
    }
}