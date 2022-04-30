using System;

namespace GameStore.Core.Models.ServiceModels.Publishers;

public class PublisherUpdateModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string HomePage { get; set; }
}