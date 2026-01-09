using System.Collections.Generic;
using NetAF.Commands;

namespace ObjectModel;

public static class CommandStore
{
    private static Dictionary<string, CustomCommand> _commands = [];

    public static void Add(string name, CustomCommand command)
    {
        _commands.Add(name, command);
    }

    public static bool TryGet(string name, out CustomCommand command)
    {
        return _commands.TryGetValue(name, out command);
    }
}