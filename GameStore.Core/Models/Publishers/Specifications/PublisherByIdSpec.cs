using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Publishers.Specifications;

public sealed class PublisherByIdSpec : Specification<Publisher>, ISingleResultSpecification
{
    public PublisherByIdSpec(Guid id)
    {
        Id = id;
        
        Query
            .Where(publisher => publisher.Id == id &&
                                publisher.IsDeleted == false);
    }

    public Guid Id { get; }
}