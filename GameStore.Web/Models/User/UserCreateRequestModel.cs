using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models.User;

public class UserCreateRequestModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(4)]
    public string UserName { get; set; }

    [Required]
    [MinLength(4)]
    public string Password { get; set; }
}