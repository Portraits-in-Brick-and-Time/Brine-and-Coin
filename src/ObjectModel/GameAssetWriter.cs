namespace ObjectModel;

using System.IO;
using Hocon;
using LibObjectFile.Elf;
using MessagePack;
using NetAF.Assets;
using NetAF.Assets.Characters;
using ObjectModel.Models;
using ObjectModel.Sections;

public class GameAssetWriter : IDisposable
{
    private readonly ElfFile _file = new(ElfArch.X86_64);
    private readonly Stream _outputStream;
    private readonly ElfStringTable _strTable = new();
    private readonly ElfSymbolTable _symbolTable = new();
    private readonly CustomSections _customSections;

    public GameAssetWriter(Stream outputStream)
    {
        _outputStream = outputStream;

        _file.Add(_strTable);
        _file.FileType = ElfFileType.Core;
        _file.Encoding = ElfEncoding.Lsb;
        _file.Version = 1;

        _customSections = new(_file);
    }

    public bool IsClosed { get; set; }

    public void WriteObjects(string defintiionFile)
    {
        var config = HoconParser.Parse(File.ReadAllText(defintiionFile));
        WriteAttributeDefinitions(config);

        foreach (var (name, def) in config.AsEnumerable().Where(_ => _.Key is not "attributes"))
        {
            var obj = def.GetObject();

            var type = obj.GetField("type").GetString();
                var description = obj.GetField("description").GetString();
            
            if (type is "character")
            {
                var isNPC = obj.GetField("isNPC").GetString() == "true";
                var model = new CharacterModel(name, description, isNPC);
                ApplyAttributes(obj, model);
                _customSections.CharactersSection.Characters.Add(model.Instanciate() as Character);
            }
            else if (type is "item")
            {
                var model = new ItemModel(name, description);
                ApplyAttributes(obj, model);
                _customSections.ItemsSection.Items.Add((Item)model.Instanciate());
                continue;
            }

        }
    }

    private void WriteAttributeDefinitions(HoconRoot config)
    {
        foreach (var (name, def) in config.AsEnumerable())
        {
            if (name is "attributes")
            {
                WriteAttributes(def);
                continue;
            }
        }
    }

    private void ApplyAttributes(HoconObject obj, GameObject model)
    {
        if (!obj.ContainsKey("attributes"))
        {
            return;
        }

        foreach (var (attrName, attrValue) in obj.GetField("attributes").GetObject().AsEnumerable())
        {
            var attrIndex = _customSections.AttributesSection.IndexOf(attrName);
            model.Attributes.Add(new IndexedRef(attrIndex), int.Parse(attrValue.GetString()));
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

            _customSections.AttributesSection.Attributes.Add(attribute);
        }
    }

    public void Close()
    {
        if (IsClosed)
        {
            return;
        }

        _customSections.Write(_symbolTable);

        _symbolTable.Link = _strTable;

        _file.Add(new ElfSectionHeaderStringTable());
        _file.Add(new ElfSectionHeaderTable());
        _file.Add(_symbolTable);
        Verify();

        _file.Write(_outputStream);

        _outputStream.Flush();
        _outputStream.Close();

        IsClosed = true;
    }

    private void Verify()
    {
        var diagnostics = _file.Verify();
        if (diagnostics.HasErrors)
        {
            foreach (var message in diagnostics.Messages)
            {
                Console.WriteLine(message);
            }
        }
    }

    public void Dispose()
    {
        Close();
    }
}
