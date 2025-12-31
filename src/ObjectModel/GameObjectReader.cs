namespace ObjectModel;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibObjectFile.Elf;
using MessagePack;
using ObjectModel.Models;
using ObjectModel.Sections;

public class GameAssetReader : IDisposable
{
    private ElfStreamSection? _objectsSection;
    private ElfFile? _file;
    private int _objectIndex;
    private Stream? _strm;
    private readonly ElfSymbolTable? _symTable;
    private readonly AttributesSection? _attributesSection;

    public GameAssetReader(Stream strm)
    {
        _file = ElfFile.Read(strm);
        _strm = strm;

        _symTable = (ElfSymbolTable)_file.Sections.First(_ => _ is ElfSymbolTable);
        _objectsSection = (ElfStreamSection)_file.Sections.First(_ => _.Name.Value == ".objects");
        _attributesSection = new((ElfStreamSection)_file.Sections.First(_ => _.Name.Value == ".attributes"));
        _attributesSection.Read();
    }

    public int Count => _symTable!.Entries.Count(s => s.Type == ElfSymbolType.Object);
    public bool HasObject => _objectIndex < Count;
    public bool IsClosed { get; set; }

    public void Close()
    {
        _file = null;
        IsClosed = true;
        _strm = null;
        _objectsSection = null;
    }

    public void Dispose()
    {
        Close();
    }

    public GameObject? ReadObject()
    {
        if (IsClosed || _strm!.Position == _strm.Length)
        {
            return null;
        }

        var instance = MessagePackSerializer.Deserialize<GameObject>(_objectsSection!.Stream);

        _objectIndex++;

        return instance;
    }
}
