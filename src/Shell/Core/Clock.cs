using System;
using System.Globalization;
using NetAF.Commands;
using NetAF.Events;
using NetAF.Logic;
using ObjectModel.Evaluation;
using Splat;

namespace BrineAndCoin.Core;

public class Clock : IEvaluable
{
    public void Step()
    {
        var currentTime = DateTime.Parse(GameExecutor.ExecutingGame.VariableManager.Get("clock.time"));
        currentTime = currentTime.AddMinutes(5);
        GameExecutor.ExecutingGame.VariableManager.Add("clock.time", currentTime.ToString());
    }

    public void Init(DateTime startTime)
    {
        var evaluator = Locator.Current.GetService<Evaluator>()!;
        evaluator.RootScope.AddOrSet("clock", this);

        Locator.Current.GetService<Game>()!.VariableManager.Add("clock.time", startTime.ToString(CultureInfo.InvariantCulture));

        EventBus.Subscribe<RegionEntered>(Handler);
        EventBus.Subscribe<RoomEntered>(Handler);
    }

    private void Handler(RegionEntered entered)
    {
        Step();
    }

    private void Handler(RoomEntered entered)
    {
        Step();
    }

    public object Evaluate(Evaluator evaluator, Scope scope)
    {
        return GameExecutor.ExecutingGame.VariableManager.Get("clock.time");
    }

    public CustomCommand CreateCommand()
    {
        return new CustomCommand(new CommandHelp("Clock", "What time is it?"), true,
            false, (game, parameters) => new Reaction(ReactionResult.Inform,
                "It is " + GameExecutor.ExecutingGame.VariableManager.Get("clock.time")));
    }

    public void Set(string time)
    {
        Locator.Current.GetService<Game>()!.VariableManager.Add("clock.time", time);
    }
}
