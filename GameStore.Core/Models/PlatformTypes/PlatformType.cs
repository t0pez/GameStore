using System;
using System.Collections.Generic;
using GameStore.Core.Models.RelationalModels;
using GameStore.SharedKernel.Interfaces;

namespace GameStore.Core.Models.PlatformTypes;

public class PlatformType : ISafeDelete
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public ICollection<GamePlatformType> Games { get; set; }

    public bool IsDeleted { get; set; }
}