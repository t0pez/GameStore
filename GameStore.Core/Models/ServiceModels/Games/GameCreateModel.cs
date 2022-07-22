using System;
using System.Collections.Generic;

namespace GameStore.Core.Models.ServiceModels.Games;

public class GameCreateModel
{
    public string Name { get; set; }
    public string Key { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public byte[] File { get; set; }
    public string PublisherName { get; set; } = string.Empty;
    
    public DateTime PublishedAt { get; set; }
    public ICollection<Guid> GenresIds { get; set; }
    public ICollection<Guid> PlatformsIds { get; set; }
}