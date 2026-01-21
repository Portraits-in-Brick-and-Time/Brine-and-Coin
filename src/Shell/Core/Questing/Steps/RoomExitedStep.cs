using NetAF.Logging.Events;

namespace BrineAndCoin.Questing.Steps;

public class RoomExitedStep(string roomName) : IQuestStep
{
    public bool IsCompleted { get; private set; }

    public void OnEvent(BaseEvent gameEvent)
    {
        if (gameEvent is RoomExited e &&
            e.Room.Identifier.Name == roomName)
        {
            IsCompleted = true;
        }
    }
}
