using MessagePack;

namespace ObjectModel;

[MessagePackObject]
public record Position([property: Key(0)] int X, [property: Key(1)] int Y, [property: Key(2)] int Z);