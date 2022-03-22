﻿using System;
using System.Collections.Generic;

namespace GameStore.Web.Models;

public class GameEditRequestModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] File { get; set; }
    public List<Guid> GenresIds { get; set; }
    public List<Guid> PlatformsIds { get; set; }
}