using System;

namespace GameStore.Web.Models.Genre;

public class GenreCreateRequestModel
{
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
}