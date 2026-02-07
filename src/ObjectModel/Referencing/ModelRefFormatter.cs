using System.Collections.Generic;
using LibObjectFile.Elf;
using MessagePack;
using MessagePack.Formatters;

namespace ObjectModel.Referencing;

[ExcludeFormatterFromSourceGeneratedResolver]
public class ModelRefFormatter : IMessagePackFormatter<ModelRef>
{
    public ElfSymbolTable SymbolTable { get; set; }

    public static readonly ModelRefFormatter Instance = new();

    public void Serialize(ref MessagePackWriter writer, ModelRef value, MessagePackSerializerOptions options)
    {
        var index = SymbolTable.Entries.FindIndex(entry => entry.Name.Value == value.Name);
        if (index == -1)
            throw new KeyNotFoundException($"ModelRef '{value.Name}' not found in symbol table.");

        writer.Write(index);
    }

    public ModelRef Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var index = reader.ReadInt32();

        return SymbolTable.Entries[index].Name.Value;
    }
}
