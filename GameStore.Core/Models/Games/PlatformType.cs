using System;
using GameStore.SharedKernel;
using GameStore.SharedKernel.Interfaces;
using System.Collections.Generic;
using GameStore.Core.Models.RelationalModels;

namespace GameStore.Core.Models.Games;

public class PlatformType : BaseEntity, ISafeDelete
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public ICollection<GamePlatformType> Games { get; set; }

    public bool IsDeleted { get; set; }
}