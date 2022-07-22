using System;
using System.Collections.Generic;
using GameStore.Core.Models.ServiceModels.Enums;

namespace GameStore.Core.Models.ServiceModels.Games;

public class GameUpdateModel
{
    public string Id { get; set; }
    public string OldGameKey { get; set; }

    public string Name { get; set; }
    public string Key { get; set; }
    public string Description { get; set; }
    public int UnitsInStock { get; set; }
    public decimal Price { get; set; }
    public bool Discounted { get; set; }
    public byte[] File { get; set; }
    public int Views { get; set; }

    public string PublisherName { get; set; }
    public ICollection<Guid> GenresIds { get; set; } = new List<Guid>();
    public ICollection<Guid> PlatformsIds { get; set; } = new List<Guid>();

    public bool? IsDeleted { get; set; }
    
    public Database Database { get; set; }

    public bool IsGameKeyChanged => OldGameKey != Key;
}