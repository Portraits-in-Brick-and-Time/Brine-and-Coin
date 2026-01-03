using System.Collections.Generic;
using System.IO;
using LibObjectFile.Elf;
using MessagePack;
using NetAF.Assets.Locations;
using ObjectModel.Models;

namespace ObjectModel.Sections;

public class RegionsSection(ElfFile file) : CustomSection(file)
{
    public List<Region> Regions { get; } = [];

    public override string Name => ".regions";
    protected override void Write(BinaryWriter writer)
    {
        writer.Write(Regions.Count);
        foreach (var region in Regions)
        {
            var start = (ulong)writer.BaseStream.Position;
            var model = RegionModel.FromRegion(region);
            writer.Write(MessagePackSerializer.Serialize(model));
            AddSymbol(region.Identifier.Name, start, (ulong)writer.BaseStream.Position - start);
        }
    }

    protected override void Read(BinaryReader reader)
    {
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var model = MessagePackSerializer.Deserialize<RegionModel>(reader.BaseStream);
            var instance = (Region)model.Instanciate(CustomSections);
            model.InstanciateAttributesTo(instance, CustomSections.AttributesSection);
            Regions.Add(instance);
        }
    }
}