using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Publishers.Specifications;

public class PublisherByIdSpec : Specification<Publisher>, ISingleResultSpecification
{
    public PublisherByIdSpec(Guid id)
    {
        Query
            .Where(publisher => publisher.Id == id &&
                                publisher.IsDeleted == false);
    }
}