using NetAF.Logging.Events;

namespace BrineAndCoin.Questing.Steps;

public class RoomEnteredStep(string roomName) : IQuestStep
{
    public bool IsCompleted { get; private set; }

    public void OnEvent(BaseEvent gameEvent)
    {
        if (gameEvent is RoomEntered e &&
            e.Room.Identifier.Name == roomName)
        {
            IsCompleted = true;
        }
    }
}
