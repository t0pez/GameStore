﻿using System;
using System.Collections.Generic;

namespace GameStore.Web.ViewModels;

public class GenreViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<GenreViewModel> SubGenres { get; set; }
}