using MediatR;

namespace GameStore.Core.Events.Notifications;

public class PublisherNameUpdatedNotification : INotification
{
    public PublisherNameUpdatedNotification(string oldName, string newName)
    {
        OldName = oldName;
        NewName = newName;
    }

    public string OldName { get; set; }
    public string NewName { get; set; }
}