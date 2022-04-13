using System;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models.Publisher;

public class PublisherEditRequestModel
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string HomePage { get; set; }
}