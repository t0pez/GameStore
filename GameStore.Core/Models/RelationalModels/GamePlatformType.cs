using System;
using GameStore.Core.Models.Games;
using GameStore.SharedKernel;

namespace GameStore.Core.Models.RelationalModels;

public class GamePlatformType : RelationshipModel
{
    public Guid GameId { get; set; }
    public Guid PlatformId { get; set; }

    public Game Game { get; set; }
    public PlatformType Platform { get; set; }
}