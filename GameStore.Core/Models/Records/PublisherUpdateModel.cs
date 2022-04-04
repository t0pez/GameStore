using System;
using System.Collections.Generic;
using GameStore.Core.Models.Games;

namespace GameStore.Core.Models.Records;

public class PublisherUpdateModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string HomePage { get; set; }

    public ICollection<Game> Games { get; set; }
}