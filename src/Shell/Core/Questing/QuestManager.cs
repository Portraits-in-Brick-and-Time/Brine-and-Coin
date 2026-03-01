using System.Collections.Generic;
using NetAF.Commands;
using NetAF.Events;
using NetAF.Logic;
using Splat;

namespace BrineAndCoin.Core.Questing;

public class QuestManager
{
    readonly Dictionary<string, Quest> _quests = [];
    private Quest? activeQuest;

    public void Init()
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

    public CustomCommand CreateCommand()
    {
        return new(new CommandHelp("Quest", "What is your current task?"), true, false, (game, parameters) =>
        {
            var message = "You have no active quest.";
            if (activeQuest != null)
            {
                message = $"{activeQuest.Name}\n{activeQuest.Description}";
            }

            return new Reaction(ReactionResult.Inform, message);
        });
    }
}
