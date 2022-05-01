using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Publishers.Specifications;

public sealed class PublisherByIdWithDetailsSpec : PublisherByIdSpec
{
    public PublisherByIdWithDetailsSpec(Guid id) : base(id)
    {
        Id = id;

        Query
            .Where(publisher => publisher.Id == id)
            .Include(publisher => publisher.Games);
    }

    public Guid Id { get; }
}