using System;

namespace GameStore.Core.Models.ServiceModels.Users;

public class UserUpdateModel
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public string UserName { get; set; }

    public string Role { get; set; }

    public string PublisherName { get; set; }
}