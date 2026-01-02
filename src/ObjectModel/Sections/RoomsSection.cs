using System.Collections.Generic;
using System.IO;
using LibObjectFile.Elf;
using MessagePack;
using NetAF.Assets.Locations;
using ObjectModel.Models;

namespace ObjectModel.Sections;

public class RoomsSection(ElfFile file) : CustomSection(file)
{
    public List<Room> Rooms { get; } = [];

    public override string Name => ".rooms";
    protected override void Write(BinaryWriter writer)
    {
        writer.Write(Rooms.Count);
        foreach (var room in Rooms)
        {
            var start = (ulong)writer.BaseStream.Position;
            var model = RoomModel.FromRoom(room);
            writer.Write(MessagePackSerializer.Serialize(model));
            AddSymbol(room.Identifier.Name, start, (ulong)writer.BaseStream.Position - start);
        }
    }

    protected override void Read(BinaryReader reader)
    {
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var model = MessagePackSerializer.Deserialize<RoomModel>(reader.BaseStream);
            var instance = (Room)model.Instanciate();
            model.InstanciateAttributesTo(instance, CustomSections.AttributesSection);
            Rooms.Add(instance);
        }
    }
}
