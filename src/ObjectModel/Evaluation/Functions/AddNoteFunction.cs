using System.Collections.Generic;
using System.Linq;
using NetAF.Logic;

namespace ObjectModel.Evaluation.Functions;

internal class AddNoteFunction : Function<string, string, object>
{
    public override string Name => "add_note";

    public override string[] Parameters => ["name", "content"];

    public override object Invoke(string name, string content)
    {
        GameExecutor.ExecutingGame?.NoteManager.Add(name, content);

        return null;
    }
}
