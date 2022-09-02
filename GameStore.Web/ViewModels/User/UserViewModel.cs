using System;

namespace GameStore.Web.ViewModels.User;

public class UserViewModel
{
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public string Role { get; set; }
}