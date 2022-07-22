namespace GameStore.Core.Models.ServiceModels.Publishers;

public class PublisherUpdateModel
{
    public string Name { get; set; }
    public string OldName { get; set; }
    public string Description { get; set; }
    public string HomePage { get; set; }

    public bool IsNameChanged => OldName != Name;
}