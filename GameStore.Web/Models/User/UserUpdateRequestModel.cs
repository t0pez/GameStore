using System;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models.User;

public class UserUpdateRequestModel
{
    public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public string OldEmail { get; set; }

    [Required]
    [MinLength(4)]
    public string UserName { get; set; }

    public string OldUserName { get; set; }

    public string Role { get; set; }

    public string PublisherName { get; set; }
}