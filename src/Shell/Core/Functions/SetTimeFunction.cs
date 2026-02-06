using BrineAndCoin.Core;
using ObjectModel.Evaluation;
using Splat;

namespace BrineAndCoin.Shell.Core.Functions;

public class SetTimeFunction : Function<string, object>
{
    public override string Name => "clock.setTime";

    public override List<string> Parameters => ["time"];

    public override object Invoke(string time)
    {
        var clock = Locator.Current.GetService<Clock>()!;

        clock.Set(time);

        return null;
    }
}