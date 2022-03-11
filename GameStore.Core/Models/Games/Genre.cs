using GameStore.SharedKernel;
using GameStore.SharedKernel.Interfaces;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameStore.Core.Models.Games;

public class Genre : BaseEntity, ISafeDelete
{
    public string Name { get; set; }

    public ICollection<Genre> SubGenres { get; set; } = new List<Genre>();
    [JsonIgnore] public ICollection<Game> Games { get; set; } = new List<Game>();
        
    public bool IsDeleted { get; set; }
}