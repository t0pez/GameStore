﻿using System;
using System.Collections.Generic;

namespace GameStore.Core.Models.Records;

public class GameUpdateModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] File { get; set; }

    public ICollection<Guid> GenresIds { get; set; } = new List<Guid>();
    public ICollection<Guid> PlatformsIds { get; set; } = new List<Guid>();

}