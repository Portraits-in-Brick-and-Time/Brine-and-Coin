using System.Collections.Generic;
using System.IO;
using LibObjectFile.Elf;
using MessagePack;
using ObjectModel.Models;

namespace ObjectModel;

internal abstract class ModelSection<T>(ElfFile file) : CustomSection(file), ISymbolTablePopulatable
    where T : GameObjectModel
{
    public List<T> Elements { get; } = [];

    public virtual void PopulateSymbolTable(ElfSymbolTable symbolTable)
    {
        for (int i = 0; i < Elements.Count; i++)
        {
            T element = Elements[i];
            AddSymbol(element.Name, (ulong)i, (ulong)i, symbolTable);
        }
    }

    protected override void Write(BinaryWriter writer)
    {
        writer.Write(MessagePackSerializer.Serialize(Elements));
    }

    protected override void Read(BinaryReader reader)
    {
        Elements.AddRange(MessagePackSerializer.Deserialize<List<T>>(reader.BaseStream));
    }
}
