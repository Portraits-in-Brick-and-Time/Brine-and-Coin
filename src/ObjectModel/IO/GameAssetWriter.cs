namespace ObjectModel.IO;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Hocon;
using LibObjectFile.Elf;
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
            if (sectionName == "meta")
            {
                WriteMeta(def.GetObject());
                continue;
            }

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

    private void WriteMeta(HoconObject hoconObject, string prefix = "")
    {
        foreach (var (key, value) in hoconObject.AsEnumerable())
        {
            string fullKey = string.IsNullOrEmpty(prefix) ? key : $"{prefix}.{key}";

            if (value.Type == HoconType.Object)
            {
                WriteMeta(value.GetObject(), fullKey);
            }
            else
            {
                _customSections.MetaSection.Properties[fullKey] = value.GetString();
            }
        }
    }

    private void ApplyAttributes(HoconObject obj, GameObjectModel model)
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

    private void ApplyInventory(HoconObject obj, IItemModel model)
    {
        if (!obj.ContainsKey("inventory"))
        {
            return;
        }

        foreach (var item in obj.GetField("inventory").GetArray())
        {
            model.Items.Add(new(item.GetString()));
        }
    }

    private void WriteCharacter(string name, HoconObject obj)
    {
        var description = obj.GetField("description").GetString();
        var isNPC = obj.GetField("isNPC").GetString() == "true";

        var model = new CharacterModel(name, description, isNPC);
        ApplyAttributes(obj, model);
        ApplyInventory(obj, model);
        _customSections.CharactersSection.Characters.Add(model);
    }

    private void WriteItem(string name, HoconObject obj)
    {
        var description = obj.GetField("description").GetString();

        var model = new ItemModel(name, description);
        ApplyAttributes(obj, model);
        _customSections.ItemsSection.Items.Add(model);
    }

    private void WriteRoom(string name, HoconObject obj)
    {
        var description = obj.GetField("description").GetString();

        var model = new RoomModel(name, description);
        ApplyAttributes(obj, model);
        ApplyInventory(obj, model);
        _customSections.RoomsSection.Rooms.Add(model);
    }

    private void WriteRegion(string name, HoconObject obj)
    {
        var description = obj.GetField("description").GetString();
        var rooms = obj.GetField("rooms").GetObject();

        var roomDict = new Dictionary<NamedRef, Position>();
        foreach (var (roomName, roomObj) in rooms)
        {
            var _obj = roomObj.GetObject();
            var nameRef = new NamedRef(roomName);
            var x = int.Parse(_obj.GetField("x").GetString());
            var y = int.Parse(_obj.GetField("y").GetString());
            var z = int.Parse(_obj.GetField("z").GetString());

            roomDict[nameRef] = new(x, y, z);
        }

        var model = new RegionModel(name, description, roomDict);

        if (obj.ContainsKey("startRoom"))
        {
            model.StartRoom = new(obj["startRoom"].GetString());
        }

        ApplyAttributes(obj, model);
        _customSections.RegionsSection.Regions.Add(model);
    }

    private void WriteAttribute(string name, HoconObject obj)
    {
        var model = new AttributeModel(
             name,
             obj.GetField("description").GetString(),
             GetOptionalFieldValue<int>(obj, "min"),
             GetOptionalFieldValue<int>(obj, "max"),
             obj.GetField("visible").GetString() == "true"
        );

        _customSections.AttributesSection.Attributes.Add(model);
    }

    private T GetOptionalFieldValue<T>(HoconObject obj, string fieldName)
        where T : IParsable<T>
    {
        return obj.ContainsKey(fieldName) ? T.Parse(obj.GetField(fieldName).GetString(), CultureInfo.InvariantCulture) : default;
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
