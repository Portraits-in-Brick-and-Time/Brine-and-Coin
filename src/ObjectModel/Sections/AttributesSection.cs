using System.Security;
using LibObjectFile.Elf;
using MessagePack;

namespace ObjectModel.Sections;

public class AttributesSection
{
    private readonly ElfStreamSection section;

    public AttributesSection(ElfStreamSection section)
    {
        this.section = section;
    }

    public Dictionary<string, NetAF.Assets.Attributes.Attribute> Attributes { get; } = [];

    public void Write()
    {
        section.Name = new ElfString(".attributes");
        var writer = new BinaryWriter(section.Stream);

        writer.Write(Attributes.Count);
        foreach (var (name, attribute) in Attributes)
        {
            var model = AttributeModel.FromAttribute(attribute);
            writer.Write(MessagePackSerializer.Serialize(model));
        }
    }

    public void Read()
    {
       var reader = new BinaryReader(section.Stream);

        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var model = MessagePackSerializer.Deserialize<AttributeModel>(section.Stream);
            Attributes.Add(model.Name, model.ToAttribute());
        }
    }
}