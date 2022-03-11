using GameStore.SharedKernel;
using GameStore.SharedKernel.Interfaces;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameStore.Core.Models.Games;

public class PlatformType : BaseEntity, ISafeDelete
{
    public string Name { get; set; }

    [JsonIgnore] public ICollection<Game> Games { get; set; }

    public bool IsDeleted { get; set; }
}