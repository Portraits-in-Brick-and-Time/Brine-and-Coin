using NetAF.Logging.Events;

namespace BrineAndCoin.Questing.Steps;

public class CharacterDiedStep(string characterName) : IQuestStep
{
    public bool IsCompleted { get; private set; }

    public void OnEvent(BaseEvent gameEvent)
    {
        if (gameEvent is CharacterDied e &&
            e.Character.Identifier.Name == characterName)
        {
            IsCompleted = true;
        }
    }
}
