namespace ObjectModel;

using System.IO;
using Hocon;
using LibObjectFile.Elf;
using MessagePack;
using ObjectModel.Models;
using ObjectModel.Sections;

public class GameAssetWriter : IDisposable
{
    private readonly MemoryStream _codeStream = new();
    private readonly ElfFile _file = new(ElfArch.ARM);
    private readonly Stream _outputStream;
    private readonly ElfStringTable _strTable = new();
    private readonly ElfSymbolTable _symbolTable = new();
    private ElfSection _objectsSection;
    private readonly AttributesSection _attributesSection;

    public GameAssetWriter(Stream outputStream)
    {
        _outputStream = outputStream;

        _file.Add(_strTable);

        var attributesStreamSection = new ElfStreamSection(ElfSectionSpecialType.Data, new MemoryStream())
        {
            Name = ".attributes",
            Flags = ElfSectionFlags.None
        };
        _attributesSection = new AttributesSection(attributesStreamSection);
        _file.Add(attributesStreamSection);
    }

    public bool IsClosed { get; set; }

    public void WriteObject(GameObject obj)
    {
        if (IsClosed)
        {
            return;
        }

        var start = (ulong)_codeStream.Position;
        MessagePackSerializer.Serialize(_codeStream, obj);
        AddSymbol(obj.Name, start, (ulong)_codeStream.Position - start);
    }

    public void WriteObjects(string defintiionFile)
    {
        var config = HoconParser.Parse(File.ReadAllText(defintiionFile));

        foreach (var (name, def) in config.AsEnumerable())
        {
            var obj = def.GetObject();

            if (name is "attributes")
            {
                WriteAttributes(def);
                continue;
            }

            var type = obj.GetField("type").GetString();
            if (type is "character")
            {
                var description = obj.GetField("description").GetString();
                var isNPC = obj.GetField("isNPC").GetString() == "true";

                WriteObject(new CharacterModel(name, description, isNPC));
            }
        }
    }

    private void WriteAttributes(HoconField def)
    {
        foreach (var (attrName, attrDef) in def.GetObject().AsEnumerable())
        {
            var obj = attrDef.GetObject();
            var attribute = new NetAF.Assets.Attributes.Attribute(attrName,
                 obj.GetField("description").GetString(),
                int.Parse(obj.GetField("min").GetString()),
                 int.Parse(obj.GetField("max").GetString()),
                 obj.GetField("visible").GetString() == "true"
            );

            _attributesSection.Attributes.Add(attrName, attribute);
        }
    }

    private void AddSymbol(string name, ulong index, ulong size)
    {
        foreach (var symbol in _symbolTable.Entries)
        {
            if (symbol.Name == name)
            {
                return;
            }
        }

        _symbolTable.Entries.Add(
            new ElfSymbol()
            {
                Bind = ElfSymbolBind.Global,
                Name = name,
                Value = index,
                Type = ElfSymbolType.Object,
                Size = size,
                SectionLink = _objectsSection
            });
    }

    public void Close()
    {
        if (IsClosed)
        {
            return;
        }

        _objectsSection = new ElfStreamSection(ElfSectionSpecialType.Text, _codeStream)
        {
            Name = ".objects",
            Flags = ElfSectionFlags.None
        };

        _attributesSection.Write();

        _symbolTable.Link = _strTable;

        _file.Add(new ElfSectionHeaderStringTable());
        _file.Add(new ElfSectionHeaderTable());

        _file.Add(_symbolTable);
        _file.Add(_objectsSection);

        _file.Write(_outputStream);
        _outputStream.Flush();

        _outputStream.Close();
        _codeStream.Close();

        IsClosed = true;
    }

    public void Dispose()
    {
        Close();
    }
}
