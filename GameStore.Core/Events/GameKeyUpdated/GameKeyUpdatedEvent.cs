using MediatR;

namespace GameStore.Core.Events.GameKeyUpdated;

public class GameKeyUpdatedEvent : INotification
{
    public GameKeyUpdatedEvent(string oldGameKey, string newGameKey)
    {
        OldGameKey = oldGameKey;
        NewGameKey = newGameKey;
    }

    public string OldGameKey { get; set; }

    public string NewGameKey { get; set; }
}