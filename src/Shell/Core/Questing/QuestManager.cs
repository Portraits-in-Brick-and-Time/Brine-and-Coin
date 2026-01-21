using BrineAndCoin.Questing;
using NetAF.Logging.Events;
using NetAF.Logic;
using Splat;

namespace BrineAndCoin.Core.Questing;

public class QuestManager
{
    readonly Dictionary<string, Quest> _quests = [];
    private Quest? activeQuest;

    public QuestManager()
    {
        var game = Locator.Current.GetService<Game>()!;
        if (game.VariableManager.ContainsVariable("activeQuest"))
        {
            ActivateQuest(game.VariableManager.Get("activeQuest"));
        }

        EventBus.Subscribe<RoomEntered>(OnEvent);
        EventBus.Subscribe<RoomExited>(OnEvent);
        EventBus.Subscribe<RegionEntered>(OnEvent);
        EventBus.Subscribe<RegionExited>(OnEvent);
        EventBus.Subscribe<ItemUsed>(OnEvent);
        EventBus.Subscribe<ItemReceived>(OnEvent);
        EventBus.Subscribe<ItemRemoved>(OnEvent);
    }

    public void AddQuest(Quest quest)
    {
        _quests[quest.Name] = quest;
    }

    public Quest? GetQuest(string name)
    {
        if (_quests.TryGetValue(name, out var quest))
        {
            return quest;
        }
        return null;
    }

    public void ActivateQuest(string name)
    {
        var quest = GetQuest(name);
        if (quest != null && quest.State == QuestState.Inactive)
        {
            quest.Start();
        }

        GameExecutor.ExecutingGame.VariableManager.Add("activeQuest", name);
        activeQuest = quest;
    }

    public Quest? ActiveQuest => activeQuest;

    void OnEvent(BaseEvent gameEvent)
    {
        activeQuest?.OnEvent(gameEvent);
    }
}