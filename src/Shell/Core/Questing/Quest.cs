using NetAF.Events;

namespace BrineAndCoin.Core.Questing;

public class Quest(string name, string description, IEnumerable<IQuestStep> steps)
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public QuestState State { get; private set; } = QuestState.Inactive;

    private readonly List<IQuestStep> _steps = [.. steps];
    private int _currentStepIndex = 0;

    public void Start()
    {
        State = QuestState.Active;
    }

    public void OnEvent(BaseEvent gameEvent)
    {
        if (State != QuestState.Active)
            return;

        var step = _steps[_currentStepIndex];
        step.OnEvent(gameEvent);

        if (step.IsCompleted)
        {
            _currentStepIndex++;
            if (_currentStepIndex >= _steps.Count)
            {
                State = QuestState.Completed;
                EventBus.Publish(new QuestCompletedEvent(Name));
            }
        }
    }
}
