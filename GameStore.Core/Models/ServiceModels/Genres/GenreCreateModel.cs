using System;

namespace GameStore.Core.Models.ServiceModels.Genres;

public class GenreCreateModel
{
    public string Name { get; set; }

    public Guid? ParentId { get; set; }
}