using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GameStore.Core.Models.ServiceModels.Enums;

namespace GameStore.Web.Models.Game;

public class GameUpdateRequestModel
{
    public string Id { get; set; }
    [Required] [MinLength(3)] public string Name { get; set; }

    [Required] [MinLength(3)] public string Key { get; set; }

    [Required] [MinLength(10)] public string Description { get; set; }

    public int UnitsInStock { get; set; }
    [Required] public decimal Price { get; set; }

    public bool Discontinued { get; set; }
    public byte[] File { get; set; }
    public string PublisherName { get; set; }
    [Required] public List<Guid> GenresIds { get; set; }
    [Required] public List<Guid> PlatformsIds { get; set; }
    [Required] public Database Database { get; set; }
}