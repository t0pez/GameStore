using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Publishers.Specifications;

public class PublisherByIdWithDetailsSpec : Specification<Publisher>, ISingleResultSpecification
{
    public PublisherByIdWithDetailsSpec(Guid id)
    {
        Query
            .Where(publisher => publisher.Id == id &&
                                publisher.IsDeleted == false)
            .Include(publisher => publisher.Games);
    }
}