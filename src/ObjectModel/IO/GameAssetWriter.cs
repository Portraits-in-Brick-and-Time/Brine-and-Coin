namespace ObjectModel.IO;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hocon;
using LibObjectFile.Elf;
using NetAF.Assets;
using NetAF.Assets.Characters;
using NetAF.Assets.Locations;
using ObjectModel.Models;

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

        _definitionWriters["attributes"] = WriteAttribute;
        _definitionWriters["characters"] = WriteCharacter;
        _definitionWriters["items"] = WriteItem;
        _definitionWriters["rooms"] = WriteRoom;
        _definitionWriters["regions"] = WriteRegion;
    }

    public bool IsClosed { get; set; }

    private readonly Dictionary<string, Action<string, HoconObject>> _definitionWriters = [];

    public void WriteObjects(string defintiionFile)
    {
        var config = HoconParser.Parse(File.ReadAllText(defintiionFile));

        foreach (var (sectionName, def) in config.AsEnumerable())
        {
            if (_definitionWriters.TryGetValue(sectionName, out var writer))
            {
                foreach (var (attrName, attrDef) in def.GetObject().AsEnumerable())
                {
                    var obj = attrDef.GetObject();
                    writer(attrName, obj);
                }
                continue;
            }

            throw new NotImplementedException($"No definition writer found for definition section '{sectionName}'");
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
            model.Attributes.Add(new NamedRef(attrName), int.Parse(attrValue.GetString()));
        }
    }

    private void WriteCharacter(string name, HoconObject obj)
    {
        var description = obj.GetField("description").GetString();
        var isNPC = obj.GetField("isNPC").GetString() == "true";

        var model = new CharacterModel(name, description, isNPC);
        ApplyAttributes(obj, model);
        _customSections.CharactersSection.Characters.Add(model.Instanciate(_customSections) as Character);
    }

    private void WriteItem(string name, HoconObject obj)
    {
        var description = obj.GetField("description").GetString();

        var model = new ItemModel(name, description);
        ApplyAttributes(obj, model);
        _customSections.ItemsSection.Items.Add((Item)model.Instanciate(_customSections));
    }

    private void WriteRoom(string name, HoconObject obj)
    {
        var description = obj.GetField("description").GetString();

        var model = new RoomModel(name, description);
        ApplyAttributes(obj, model);
        _customSections.RoomsSection.Rooms.Add((Room)model.Instanciate(_customSections));
    }

    private void WriteRegion(string name, HoconObject obj)
    {
        var description = obj.GetField("description").GetString();
        var rooms = obj.GetField("rooms").GetArray().Select(r => r.GetObject()).ToArray();

        var roomDict = new Dictionary<NamedRef, Position>();
        foreach (var room in rooms)
        {
            var nameRef = new NamedRef(room.GetField("name").GetString());
            var x = int.Parse(room.GetField("x").GetString());
            var y = int.Parse(room.GetField("y").GetString());
            var z = int.Parse(room.GetField("z").GetString());
            
            roomDict[nameRef] = new(x, y, z);
        }

        var model = new RegionModel(name, description, roomDict);
        ApplyAttributes(obj, model);
        _customSections.RegionsSection.Regions.Add((Region)model.Instanciate(_customSections));
    }

    private void WriteAttribute(string name, HoconObject obj)
    {
        var attribute = new NetAF.Assets.Attributes.Attribute(name,
             obj.GetField("description").GetString(),
            int.Parse(obj.GetField("min").GetString()),
             int.Parse(obj.GetField("max").GetString()),
             obj.GetField("visible").GetString() == "true"
        );

        _customSections.AttributesSection.Attributes.Add(attribute);
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
