using System.Collections.Generic;
using System.IO;
using LibObjectFile.Elf;
using MessagePack;
using ObjectModel.Models;

namespace ObjectModel.Sections;

internal class RoomsSection(ElfFile file) : CustomSection(file)
{
    public List<RoomModel> Rooms { get; } = [];

    public override string Name => ".rooms";
    protected override void Write(BinaryWriter writer)
    {
        foreach (var room in Rooms)
        {
            var start = (ulong)writer.BaseStream.Position;
            writer.Write(MessagePackSerializer.Serialize(room));

            AddSymbol(room.Name, start, (ulong)writer.BaseStream.Position - start);
        }
    }

    protected override void Read(BinaryReader reader)
    {
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            var model = MessagePackSerializer.Deserialize<RoomModel>(reader.BaseStream);
            Rooms.Add(model);
        }
    }
}
