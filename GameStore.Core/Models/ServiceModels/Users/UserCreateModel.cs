using System;

namespace GameStore.Core.Models.ServiceModels.Users;

public class UserCreateModel
{
    public string Email { get; set; }

    public string UserName { get; set; }

    public Guid CookieUserId { get; set; }

    public string Password { get; set; }
}