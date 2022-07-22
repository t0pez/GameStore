using MediatR;

namespace GameStore.Core.Events.Notifications;

public class GameKeyUpdatedNotification : INotification
{
    public GameKeyUpdatedNotification(string oldGameKey, string newGameKey)
    {
        OldGameKey = oldGameKey;
        NewGameKey = newGameKey;
    }

    public string OldGameKey { get; set; }
    public string NewGameKey { get; set; }
}