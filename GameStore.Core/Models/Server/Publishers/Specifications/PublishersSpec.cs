using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Server.Publishers.Specifications;

public class PublishersSpec : SafeDeleteSpec<Publisher>
{
    public PublishersSpec ById(Guid id)
    {
        Query
           .Where(publisher => publisher.Id == id);

        return this;
    }

    public PublishersSpec ByName(string name)
    {
        Query
           .Where(publisher => publisher.Name == name);

        return this;
    }

    public PublishersSpec WithDetails()
    {
        Query
           .Include(publisher => publisher.Games);

        return this;
    }
}