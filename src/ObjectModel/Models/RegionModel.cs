using System.Collections.Generic;
using MessagePack;
using NetAF.Assets;
using NetAF.Assets.Locations;

namespace ObjectModel.Models;

#nullable enable

[MessagePackObject]
public class RegionModel : GameObject
{
    [Key(3)]
    public Dictionary<NamedRef, Position> Rooms { get; set; }

    [Key(4)]
    public NamedRef? StartRoom { get; set; }

    public RegionModel()
    {
        Rooms = [];
    }
    
    public RegionModel(string name, string description, Dictionary<NamedRef, Position> rooms, NamedRef? startRoom = null)
    {
        Name = name;
        Description = description;
        Rooms = rooms;
        StartRoom = startRoom;
    }

    public static RegionModel FromRegion(Region region)
    {
        return new RegionModel(region.Identifier.Name, region.Description.GetDescription(), [], null);
    }

    public override IExaminable Instanciate(CustomSections customSections)
    {
        var region = new Region(Name, Description);
        foreach (var (roomRef, (x, y, z)) in Rooms)
        {
            region.AddRoom(customSections.RoomsSection.GetByName(roomRef.Name), x, y, z);
        }

        if(StartRoom is not null)
        {
            region.SetStartRoom(customSections.RoomsSection.GetByName(StartRoom?.Name));
        }

        return region;
    }
}