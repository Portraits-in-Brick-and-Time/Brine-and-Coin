using System;
using Hocon;
using MessagePack;

namespace ObjectModel;

[MessagePackObject]
public record Position([property: Key(0)] int X, [property: Key(1)] int Y, [property: Key(2)] int Z)
{
    public static Position Parse(HoconField field)
    {
        if (field.Type == HoconType.Object)
        {
            var obj = field.GetObject();

            var x = int.Parse(obj.GetField("x").GetString());
            var y = int.Parse(obj.GetField("y").GetString());
            var z = int.Parse(obj.GetField("z").GetString());

            return new(x, y, z);
        }
        if (field.Type == HoconType.String)
        {
            var value = field.GetString();
            var parts = value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var x = int.Parse(parts[0]);
            var y = int.Parse(parts[1]);
            var z = int.Parse(parts[2]);

            return new(x, y, z);
        }

        throw new InvalidOperationException($"Invalid Position at {field.Path}");
    }
}