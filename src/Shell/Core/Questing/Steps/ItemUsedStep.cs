using NetAF.Events;

namespace BrineAndCoin.Core.Questing.Steps;

public class ItemUsedStep(string itemName) : IQuestStep
{
    public bool IsCompleted { get; private set; }

    public void OnEvent(BaseEvent gameEvent)
    {
        if (gameEvent is ItemUsed e &&
            e.Item.Identifier.Name == itemName)
        {
            IsCompleted = true;
        }
    }
}
