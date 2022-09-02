using System;
using System.Collections.Generic;
using GameStore.Core.Models.Server.RelationalModels;
using GameStore.SharedKernel.Interfaces;

namespace GameStore.Core.Models.Server.PlatformTypes;

public class PlatformType : ISafeDelete
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public ICollection<GamePlatformType> Games { get; set; }

    public bool IsDeleted { get; set; }
}