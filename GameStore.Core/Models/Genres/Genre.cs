using System;
using System.Collections.Generic;
using GameStore.Core.Models.RelationalModels;
using GameStore.SharedKernel.Interfaces;

namespace GameStore.Core.Models.Genres;

public class Genre : ISafeDelete
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? CategoryId { get; set; }

    public Guid? ParentId { get; set; }
    public Genre Parent { get; set; }
    
    public ICollection<Genre> SubGenres { get; set; } = new List<Genre>();
    public ICollection<GameGenre> Games { get; set; } = new List<GameGenre>();
        
    public bool IsDeleted { get; set; }
}