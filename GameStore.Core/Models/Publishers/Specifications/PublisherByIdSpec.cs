using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Publishers.Specifications;

public class PublisherByIdSpec : SingleResultSafeDeleteSpec<Publisher>
{
    public PublisherByIdSpec(Guid id)
    {
        Id = id;

        Query
            .Where(publisher => publisher.Id == id);
    }

    public Guid Id { get; }
}