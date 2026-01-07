using System.Collections.Generic;
using System.IO;
using LibObjectFile.Elf;
using MessagePack;

namespace ObjectModel.Sections;

internal class AttributesSection(ElfFile file) : ModelSection<AttributeModel>(file)
{
    public override string Name => ".attributes";

    protected override ElfSymbolType SymbolType => ElfSymbolType.Common;

    public int IndexOf(string name)
    {
        for (var i = 0; i < Elements.Count; i++)
        {
            if (Elements[i].Name == name)
            {
                return i;
            }
        }

        throw new KeyNotFoundException($"Attribute '{name}' not found.");
    }
}
