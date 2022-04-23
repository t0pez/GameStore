using System;

namespace GameStore.Core.Models.ServiceModels.Genres;

public class GenreUpdateModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
}