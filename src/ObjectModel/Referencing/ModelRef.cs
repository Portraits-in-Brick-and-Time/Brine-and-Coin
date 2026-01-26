using MessagePack;

namespace ObjectModel.Referencing;

[MessagePackObject]
public record ModelRef([property: Key(0)] string Name)
{
    public static implicit operator string(ModelRef modelRef) => modelRef.Name;
    public static implicit operator ModelRef(string name) => new(name);
    public override string ToString() => Name;
}