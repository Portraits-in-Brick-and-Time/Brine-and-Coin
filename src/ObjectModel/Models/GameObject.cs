using MessagePack;

namespace ObjectModel.Models;

[Union(0, typeof(Models.CharacterModel))]
[MessagePackObject]
public abstract class GameObject
{
    [Key(0)]
    public string Name { get; set; }

    public abstract object Instanciate();

    public TOutput? Instanciate<TOutput>()
        where TOutput : class
    {
        return Instanciate() as TOutput;
    }
}
