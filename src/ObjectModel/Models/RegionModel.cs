using System.Collections.Generic;
using MessagePack;
using ObjectModel.Referencing;

namespace ObjectModel.Models;

#nullable enable

[MessagePackObject(AllowPrivate = true)]
internal class RegionModel : GameObjectModel
{
    [Key(4)]
    public Dictionary<ModelRef, Position> Rooms { get; set; }

    [Key(5)]
    public ModelRef? StartRoom { get; set; }

    public RegionModel()
    {
        Rooms = [];
    }

    public RegionModel(string name, string description, Dictionary<ModelRef, Position> rooms, ModelRef? startRoom = null)
    {
        Name = name;
        Description = description;
        Rooms = rooms;
        StartRoom = startRoom;
    }
}