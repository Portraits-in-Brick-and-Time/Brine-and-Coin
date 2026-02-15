using System.Collections.Generic;
using NetAF.Assets;

namespace ObjectModel.Evaluation.Functions;

internal class InteractionFunction : Function<InteractionResult, string, Interaction>
{
    public override string Name => "interaction";

    public override string[] Parameters => ["result", "description"];

    public override Interaction Invoke(InteractionResult result, string description)
    {
        return new Interaction(result, null, description);
    }
}
