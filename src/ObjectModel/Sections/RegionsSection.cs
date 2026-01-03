using System.Collections.Generic;
using System.IO;
using LibObjectFile.Elf;
using MessagePack;
using ObjectModel.Models;

namespace ObjectModel.Sections;

internal class RegionsSection(ElfFile file) : CustomSection(file)
{
    public List<RegionModel> Regions { get; } = [];

    public override string Name => ".regions";
    protected override void Write(BinaryWriter writer)
    {
        foreach (var region in Regions)
        {
            var start = (ulong)writer.BaseStream.Position;
            writer.Write(MessagePackSerializer.Serialize(region));

            AddSymbol(region.Name, start, (ulong)writer.BaseStream.Position - start);
        }
    }

    protected override void Read(BinaryReader reader)
    {
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            var model = MessagePackSerializer.Deserialize<RegionModel>(reader.BaseStream);
            Regions.Add(model);
        }
    }
}