using MessagePack;

namespace ObjectModel;

[MessagePackObject]
public record IndexedRef([property: Key(0)] int Index);