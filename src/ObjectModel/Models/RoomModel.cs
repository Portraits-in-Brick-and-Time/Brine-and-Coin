using System.Collections.Generic;
using MessagePack;
using NetAF.Assets;
using NetAF.Assets.Locations;

namespace ObjectModel.Models;

[MessagePackObject]
public class RoomModel : GameObject
{
    [Key(3)]
    public List<NamedRef> Items { get; set; }

    public RoomModel(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public static RoomModel FromRoom(Room room)
    {
        return new RoomModel(room.Identifier.Name, room.Description.GetDescription());
    }

    public override IExaminable Instanciate(CustomSections customSections)
    {
        return new Room(Name, Description);
    }
}
