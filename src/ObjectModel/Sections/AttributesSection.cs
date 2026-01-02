using System.Collections.Generic;
using System.IO;
using LibObjectFile.Elf;
using MessagePack;
using NetAF.Assets.Attributes;

namespace ObjectModel.Sections;

public class AttributesSection(ElfFile file) : CustomSection(file)
{
    public List<Attribute> Attributes { get; } = [];

    public override string Name => ".attributes";

    protected override void Write(BinaryWriter writer)
    {
        writer.Write(Attributes.Count);
        foreach (var attribute in Attributes)
        {
            var model = AttributeModel.FromAttribute(attribute);
            writer.Write(MessagePackSerializer.Serialize(model));
        }
    }

    protected override void Read(BinaryReader reader)
    {
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var model = MessagePackSerializer.Deserialize<AttributeModel>(reader.BaseStream);
            Attributes.Add(model.ToAttribute());
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

    public Attribute GetAttributeByName(string name)
    {
        foreach (var attribute in Attributes)
        {
            if (attribute.Name == name)
            {
                return attribute;
            }
        }

        throw new KeyNotFoundException($"Attribute '{name}' not found.");
    }
}
