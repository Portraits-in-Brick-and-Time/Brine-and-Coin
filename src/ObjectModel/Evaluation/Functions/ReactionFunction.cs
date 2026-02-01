using System;
using System.Collections.Generic;
using NetAF.Commands;

namespace ObjectModel.Evaluation.Functions;

internal class ReactionFunction : Function<string, string, Reaction>
{
    public override string Name => "reaction";

    public override List<string> Parameters => ["type", "description"];

    public override Reaction Invoke(string type, string description)
    {
        return new Reaction(Enum.Parse<ReactionResult>(type), description);
    }
}