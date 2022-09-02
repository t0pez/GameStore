using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Server.Users.Specifications;

public class UsersSpec : SafeDeleteSpec<User>
{
    public UsersSpec ById(Guid id)
    {
        Query
           .Where(user => user.Id == id);

        return this;
    }

    public UsersSpec ByEmail(string email)
    {
        Query
           .Where(user => user.Email == email);

        return this;
    }

    public UsersSpec ByUserName(string userName)
    {
        Query
           .Where(user => user.UserName == userName);

        return this;
    }

    public UsersSpec ByPublisherName(string publisherName)
    {
        Query
           .Where(user => user.PublisherName == publisherName);

        return this;
    }
}