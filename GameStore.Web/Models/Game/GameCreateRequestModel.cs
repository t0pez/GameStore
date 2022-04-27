using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models.Game;

public class GameCreateRequestModel
{
    [Required]
    [MinLength(5)]
    public string Name { get; set; }
    [Required]
    [MinLength(5)]
    public string Key { get; set; }
    [Required]
    [MinLength(10)]
    public string Description { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public Guid PublisherId { get; set; }
    [Required]
    public ICollection<Guid> GenresIds { get; set; }
    [Required]
    public ICollection<Guid> PlatformsIds { get; set; }
}