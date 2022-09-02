using MediatR;

namespace GameStore.Core.Events.PublisherNameUpdated;

public class PublisherNameUpdatedEvent : INotification
{
    public PublisherNameUpdatedEvent(string oldName, string newName)
    {
        OldName = oldName;
        NewName = newName;
    }

    public string OldName { get; set; }

    public string NewName { get; set; }
}