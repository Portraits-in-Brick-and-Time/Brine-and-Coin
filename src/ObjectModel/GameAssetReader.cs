namespace ObjectModel;

using System;
using System.IO;
using System.Linq;
using LibObjectFile.Elf;
using MessagePack;
using NetAF.Assets;
using ObjectModel.Models;

public class GameAssetReader
{
    private ElfFile _file;

    public readonly ElfSymbolTable SymbolTable;
    public readonly CustomSections CustomSections;

    public ElfFile File { get => _file; }

    public GameAssetReader(Stream strm)
    {
        _file = ElfFile.Read(strm);

        SymbolTable = (ElfSymbolTable)File.Sections.First(_ => _ is ElfSymbolTable);

        CustomSections = new(File);
        CustomSections.Read();
    }
}
