using NetAF.Commands;
using NetAF.Logging.Events;
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

        Locator.Current.GetService<Game>()!.VariableManager.Add("clock.time", startTime.ToString());

        EventBus.Subscribe<RegionEntered>(handler);
        EventBus.Subscribe<RoomEntered>(handler);
    }

    private void handler(RegionEntered entered)
    {
        Step();
    }

    private void handler(RoomEntered entered)
    {
        Step();
    }

    public object Evaluate(Evaluator evaluator, Scope scope)
    {
        return GameExecutor.ExecutingGame.VariableManager.Get("clock.time");
    }

    public CustomCommand CreateCommand()
    {
        return new(new CommandHelp("Clock", "What time is it?"), true, false, (game, parameters) =>
        {
            return new Reaction(ReactionResult.Inform, "It is " + GameExecutor.ExecutingGame.VariableManager.Get("clock.time"));
        });
    }
}