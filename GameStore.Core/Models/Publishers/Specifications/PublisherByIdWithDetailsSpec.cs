using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Publishers.Specifications;

public sealed class PublisherByIdWithDetailsSpec : PublisherByIdSpec
{
    public PublisherByIdWithDetailsSpec(Guid id) : base(id)
    {
        Query
            .Include(publisher => publisher.Games);
    }
}