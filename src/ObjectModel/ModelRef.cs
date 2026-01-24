using MessagePack;

namespace ObjectModel;

[MessagePackObject]
public record ModelRef([property: Key(0)] string Name);