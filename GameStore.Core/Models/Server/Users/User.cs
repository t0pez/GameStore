using System;
using GameStore.SharedKernel.Interfaces;

namespace GameStore.Core.Models.Server.Users;

public class User : ISafeDelete
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public string UserName { get; set; }

    public string PasswordHash { get; set; }

    public string Role { get; set; }

    public string PublisherName { get; set; }

    public bool IsDeleted { get; set; }
}