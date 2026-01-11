using MessagePack;
using NetAF.Assets.Locations;

namespace ObjectModel.Models;

[MessagePackObject(AllowPrivate = true)]
internal class ExitModel : GameObjectModel
{
    [Key(4)]
    public Direction Direction { get; set; }

    [Key(5)]
    public bool IsLocked { get; set; }
}