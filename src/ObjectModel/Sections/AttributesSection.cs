using System.Collections.Generic;
using System.IO;
using LibObjectFile.Elf;
using MessagePack;

namespace ObjectModel.Sections;

internal class AttributesSection(ElfFile file) : CustomSection(file)
{
    public List<AttributeModel> Attributes { get; } = [];

    public override string Name => ".attributes";

    protected override void Write(BinaryWriter writer)
    {
        foreach (var attribute in Attributes)
        {
            writer.Write(MessagePackSerializer.Serialize(attribute));
        }
    }

    protected override void Read(BinaryReader reader)
    {
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            var model = MessagePackSerializer.Deserialize<AttributeModel>(reader.BaseStream);
            Attributes.Add(model);
        }
    }

    public int IndexOf(string name)
    {
        for (var i = 0; i < Attributes.Count; i++)
        {
            if (Attributes[i].Name == name)
            {
                return i;
            }
        }

        throw new KeyNotFoundException($"Attribute '{name}' not found.");
    }
}
