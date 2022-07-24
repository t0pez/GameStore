using System;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models.Genre;

public class GenreCreateRequestModel
{
    [Required]
    public string Name { get; set; }

    public Guid? ParentId { get; set; }
}