using System;
using System.Collections.Generic;
using GameStore.Core.Models.Games;
using GameStore.SharedKernel.Interfaces;

namespace GameStore.Core.Models.Publishers;

public class Publisher : ISafeDelete
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string HomePage { get; set; }

    public ICollection<Game> Games { get; set; } // TODO: change to GamePublisher or smth
    
    public bool IsDeleted { get; set; }
}