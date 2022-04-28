using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models.Game;

public class GameUpdateRequestModel
{
    public Guid Id { get; set; }
    [Required] [MinLength(5)] public string Name { get; set; }
    [Required] [MinLength(5)] public string Key { get; set; }
    [Required] [MinLength(10)] public string Description { get; set; }
    public int UnitsInStock { get; set; }
    public decimal Price { get; set; }
    public bool Discounted { get; set; }
    public byte[] File { get; set; }
    public Guid PublisherId { get; set; }
    [Required] public List<Guid> GenresIds { get; set; }
    [Required] public List<Guid> PlatformsIds { get; set; }
}