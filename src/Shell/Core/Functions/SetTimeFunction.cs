using System.Collections.Generic;
using ObjectModel.Evaluation;
using Splat;

namespace BrineAndCoin.Core.Functions;

public class SetTimeFunction : Function<string, object>
{
    public override string Name => "clock.setTime";

    public override string[] Parameters => ["time"];

    public override object Invoke(string time)
    {
        var clock = Locator.Current.GetService<Clock>()!;

        clock.Set(time);

        return null;
    }
}
