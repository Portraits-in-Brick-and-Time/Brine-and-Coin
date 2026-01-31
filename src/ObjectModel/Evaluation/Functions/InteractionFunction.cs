using System;
using System.Collections.Generic;
using NetAF.Assets;

namespace ObjectModel.Evaluation.Functions;

internal class InteractionFunction : Function<string, string, Interaction>
{
    public override string Name => "interaction";

    public override List<string> Parameters => ["result", "description"];

    public override Interaction Invoke(string result, string description)
    {
        //Todo: automatic marshalling of enums when other value types are supported
        return new Interaction(Enum.Parse<InteractionResult>(result), null, description);
    }
}