using System.Collections.Generic;
using NetAF.Commands;

namespace ObjectModel.Evaluation.Functions;

internal class ReactionFunction : Function<ReactionResult, string, Reaction>
{
    public override string Name => "reaction";

    public override string[] Parameters => ["type", "description"];

    public override Reaction Invoke(ReactionResult type, string description)
    {
        return new Reaction(type, description);
    }
}