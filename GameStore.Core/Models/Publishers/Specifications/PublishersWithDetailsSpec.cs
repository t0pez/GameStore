using Ardalis.Specification;

namespace GameStore.Core.Models.Publishers.Specifications;

public sealed class PublishersWithDetailsSpec : Specification<Publisher>
{
    public PublishersWithDetailsSpec()
    {
        Query
            .Where(publisher => publisher.IsDeleted == false);
    }
}