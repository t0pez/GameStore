using System;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models.Genre;

public class GenreUpdateRequestModel
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    [Required] public string Name { get; set; }
}