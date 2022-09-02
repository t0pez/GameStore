using System;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.PlatformTypes;

namespace GameStore.Core.Models.Server.RelationalModels;

public class GamePlatformType
{
    public Guid GameId { get; set; }

    public Guid PlatformId { get; set; }

    public Game Game { get; set; }

    public PlatformType Platform { get; set; }
}