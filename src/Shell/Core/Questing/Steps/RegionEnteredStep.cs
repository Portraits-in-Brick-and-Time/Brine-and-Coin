using NetAF.Events;

namespace BrineAndCoin.Core.Questing.Steps;

public class RegionEnteredStep(string regionName) : IQuestStep
{
    public bool IsCompleted { get; private set; }

    public void OnEvent(BaseEvent gameEvent)
    {
        if (gameEvent is RegionEntered e &&
            e.Region.Identifier.Name == regionName)
        {
            IsCompleted = true;
        }
    }
}
