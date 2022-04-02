using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models;

public class GameCreateRequestModel
{
    [Required]
    [MinLength(5)]
    public string Name { get; set; }
    [Required]
    [MinLength(10)]
    public string Description { get; set; }
    [Required]
    public decimal Price { get; set; }
}