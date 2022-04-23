using System;

namespace GameStore.Web.Models.Genre;

public class GenreUpdateRequestModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
}