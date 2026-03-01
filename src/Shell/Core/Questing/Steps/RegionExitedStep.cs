using NetAF.Events;

namespace BrineAndCoin.Core.Questing.Steps;

public class RegionExitedStep(string regionName) : IQuestStep
{
    public bool IsCompleted { get; private set; }

    public void OnEvent(BaseEvent gameEvent)
    {
        if (gameEvent is RegionExited e &&
            e.Region.Identifier.Name == regionName)
        {
            IsCompleted = true;
        }
    }
}
