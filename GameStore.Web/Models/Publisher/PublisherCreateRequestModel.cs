using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models.Publisher;

public class PublisherCreateRequestModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string HomePage { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string ContactName { get; set; }

    [Required]

    public string ContactTitle { get; set; }

    [Required]

    public string Phone { get; set; }

    [Required]
    public string Fax { get; set; }

    [Required]
    public string Country { get; set; }

    [Required]
    public string PostalCode { get; set; }

    [Required]
    public string Region { get; set; }
}