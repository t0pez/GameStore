using System;
using System.Collections.Generic;

namespace GameStore.Core.Models.ServiceModels.Games;

public class GameUpdateModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Key { get; set; }
    public string Description { get; set; }
    public int UnitsInStock { get; set; }
    public decimal Price { get; set; }
    public bool Discounted { get; set; }
    public byte[] File { get; set; }

    public Guid PublisherId { get; set; }
    public ICollection<Guid> GenresIds { get; set; } = new List<Guid>();
    public ICollection<Guid> PlatformsIds { get; set; } = new List<Guid>();

}