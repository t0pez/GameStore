using System;
using GameStore.SharedKernel.Interfaces;
using System.Collections.Generic;
using GameStore.Core.Models.RelationalModels;

namespace GameStore.Core.Models.Games;

public class Genre : ISafeDelete
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public Guid? ParentId { get; set; }
    public Genre Parent { get; set; }
    
    public ICollection<Genre> SubGenres { get; set; } = new List<Genre>();
    public ICollection<GameGenre> Games { get; set; } = new List<GameGenre>();
        
    public bool IsDeleted { get; set; }
}