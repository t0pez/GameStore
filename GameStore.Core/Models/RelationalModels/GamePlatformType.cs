﻿using System;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.PlatformTypes;

namespace GameStore.Core.Models.RelationalModels;

public class GamePlatformType
{
    public Guid GameId { get; set; }
    public Guid PlatformId { get; set; }

    public Game Game { get; set; }
    public PlatformType Platform { get; set; }
}