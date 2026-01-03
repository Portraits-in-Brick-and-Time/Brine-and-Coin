using System.Collections.Generic;
using System.IO;
using LibObjectFile.Elf;
using MessagePack;
using ObjectModel.Models;

namespace ObjectModel.Sections;

internal class ItemsSection(ElfFile file) : CustomSection(file)
{
    public List<ItemModel> Items { get; } = [];

    public override string Name => ".items";
    protected override void Write(BinaryWriter writer)
    {
        foreach (var item in Items)
        {
            var start = (ulong)writer.BaseStream.Position;
            writer.Write(MessagePackSerializer.Serialize(item));

            AddSymbol(item.Name, start, (ulong)writer.BaseStream.Position - start);
        }
    }

    protected override void Read(BinaryReader reader)
    {
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            var model = MessagePackSerializer.Deserialize<ItemModel>(reader.BaseStream);
            Items.Add(model);
        }
    }
}
