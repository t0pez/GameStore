using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Publishers.Specifications;

public sealed class PublisherByIdWithDetailsSpec : Specification<Publisher>, ISingleResultSpecification
{
    public PublisherByIdWithDetailsSpec(Guid id)
    {
        Id = id;
        
        Query
            .Where(publisher => publisher.Id == id &&
                                publisher.IsDeleted == false)
            .Include(publisher => publisher.Games);
    }

    public Guid Id { get; }
}