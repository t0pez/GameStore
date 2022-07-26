using GameStore.Core.Models.ServiceModels.Enums;

namespace GameStore.Web.ViewModels.Publisher;

public class PublisherViewModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string HomePage { get; set; }
    public string Address { get; set; }
    public string City { get; set; }

    public string ContactName { get; set; }
    public string ContactTitle { get; set; }
    public string Phone { get; set; }
    public string Fax { get; set; }

    public string Country { get; set; }
    public string PostalCode { get; set; }
    public string Region { get; set; }

    public Database Database { get; set; }
    public bool IsDeleted { get; set; }
}