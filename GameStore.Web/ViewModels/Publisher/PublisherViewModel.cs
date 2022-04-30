using System;

namespace GameStore.Web.ViewModels.Publisher;

public class PublisherViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string HomePage { get; set; }
}