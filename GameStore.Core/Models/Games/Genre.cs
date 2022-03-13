using System;
using GameStore.SharedKernel;
using GameStore.SharedKernel.Interfaces;
using System.Collections.Generic;

namespace GameStore.Core.Models.Games;

public class Genre : BaseEntity, ISafeDelete
{
    public string Name { get; set; }

    public Guid? ParentId { get; set; }
    public Genre Parent { get; set; }
    
    public ICollection<Genre> SubGenres { get; set; } = new List<Genre>();
    public ICollection<Game> Games { get; set; } = new List<Game>();
        
    public bool IsDeleted { get; set; }
}