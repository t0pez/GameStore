﻿using System;
using System.Collections.Generic;

namespace GameStore.Web.ViewModels;

public class GameViewModel
{
    public Guid Id { get; set; }
    
    public string Key { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public List<GenreViewModel> Genres { get; set; }
    public List<PlatformTypeViewModel> Platforms { get; set; }
}