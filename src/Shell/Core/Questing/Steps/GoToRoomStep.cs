using NetAF.Events;

namespace BrineAndCoin.Core.Questing.Steps;

public class GoToRoomStep(string roomName) : IQuestStep
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
