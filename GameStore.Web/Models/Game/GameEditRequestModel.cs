using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models.Game;

public class GameEditRequestModel
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    [MinLength(5)]
    public string Name { get; set; }
    [Required]
    [MinLength(10)]
    public string Description { get; set; }
    [Required]
    public int UnitsInStock { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public bool Discounted { get; set; }
    [Required]
    public byte[] File { get; set; }
    [Required]
    public List<Guid> GenresIds { get; set; }
    [Required]
    public List<Guid> PlatformsIds { get; set; }
}