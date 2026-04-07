namespace ObjectModel.IO;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Hocon;
using LibObjectFile.Elf;
using MessagePack;
using MessagePack.Resolvers;
using NetAF.Assets.Locations;
using NiL.JS.Core;
using ObjectModel.Models;
using ObjectModel.Parsing;
using ObjectModel.Referencing;

public class GameAssetWriter : IDisposable
{
    private readonly ElfFile _file = new(ElfArch.X86_64);
    private readonly Stream _outputStream;
    private readonly ElfStringTable _strTable = new();
    private readonly ElfSymbolTable _symbolTable = new();
    private readonly CustomSections _customSections;

    private readonly Context _definitionContext = new();

    static GameAssetWriter()
    {
        Parser.DefineCustomCodeFragment(typeof(ItemDefinition));
        Parser.DefineCustomCodeFragment(typeof(AttributeDefinition));
        Parser.DefineCustomCodeFragment(typeof(MetaDefinition));
    }

    public GameAssetWriter(Stream outputStream)
    {
        _outputStream = outputStream;

        _file.Add(_strTable);
        _file.FileType = ElfFileType.Core;
        _file.Encoding = ElfEncoding.Lsb;
        _file.Version = 2;

        _customSections = new(_file);

        _definitionWriters["characters"] = WriteCharacter;
        _definitionWriters["rooms"] = WriteRoom;
        _definitionWriters["regions"] = WriteRegion;
        _definitionWriters["quests"] = WriteQuest;

        ModelRefFormatter.Instance.SymbolTable = _symbolTable;
        var resolver = CompositeResolver.Create(
                    new ModelRefResolver(),
                    StandardResolver.Instance
                );

        MessagePackSerializer.DefaultOptions = MessagePackSerializer.DefaultOptions
            .WithResolver(resolver)
            .WithCompression(MessagePackCompression.Lz4Block);
    }

    /*
        private void WriteFunction(string name, HoconObject definition)
        {
            var funcDef = new FuncDefModel
            {
                Name = name,
                Parameters = definition.GetField("params")
                                .GetArray()
                                .Select(_ => _.GetString()).ToArray()
            };

            //ApplyCode(definition, funcDef.Action, "do");

            _customSections.FunctionDefinitionsSection.Elements.Add(funcDef);
        }
        */

    private void WriteQuest(string name, HoconObject obj)
    {
        var description = obj.ContainsKey("description") ? obj.GetField("description").GetString() : string.Empty;

        var model = new Models.Quest.QuestModel
        {
            Name = name,
            Description = description
        };

        if (obj.ContainsKey("steps"))
        {
            foreach (var step in obj.GetField("steps").GetArray())
            {
                if (step.Type == HoconType.Object)
                {
                    var stepObj = step.GetObject();
                    var type = stepObj.GetField("type").GetString();

                    switch (type)
                    {
                        case "GoToRoom":
                            model.Steps.Add(new Models.Quest.Steps.GoToRoomStepModel(stepObj.GetField("roomName").GetString()));
                            break;
                        case "CharacterDies":
                            model.Steps.Add(new Models.Quest.Steps.CharacterDiesStepModel(stepObj.GetField("characterName").GetString()));
                            break;
                        case "ItemReceived":
                            model.Steps.Add(new Models.Quest.Steps.ItemReceivedStepModel(stepObj.GetField("itemName").GetString()));
                            break;
                        case "ItemRemoved":
                            model.Steps.Add(new Models.Quest.Steps.ItemRemovedStepModel(stepObj.GetField("itemName").GetString()));
                            break;
                        case "ItemUsed":
                            model.Steps.Add(new Models.Quest.Steps.ItemUsedStepModel(stepObj.GetField("itemName").GetString()));
                            break;
                        case "RegionEntered":
                            model.Steps.Add(new Models.Quest.Steps.RegionEnteredStepModel(stepObj.GetField("regionName").GetString()));
                            break;
                        case "RegionExited":
                            model.Steps.Add(new Models.Quest.Steps.RegionExitedStepModel(stepObj.GetField("regionName").GetString()));
                            break;
                        case "RoomEntered":
                            model.Steps.Add(new Models.Quest.Steps.RoomEnteredStepModel(stepObj.GetField("roomName").GetString()));
                            break;
                        case "RoomExited":
                            model.Steps.Add(new Models.Quest.Steps.RoomExitedStepModel(stepObj.GetField("roomName").GetString()));
                            break;
                        default:
                            // ignore unknown step types
                            break;
                    }
                }
            }
        }

        _customSections.QuestsSection.Elements.Add(model);
    }

    public bool IsClosed { get; set; }

    private readonly Dictionary<string, Action<string, HoconObject>> _definitionWriters = [];

    public void WriteObjects(string defintionFile)
    {
        if (defintionFile.EndsWith("items.conf")
        || defintionFile.EndsWith("attributes.conf")
        || defintionFile.EndsWith("meta.conf"))
        {
            _definitionContext.Eval(File.ReadAllText(defintionFile));
        }
        PropagateModels();
        return;

        var config = HoconParser.Parse(File.ReadAllText(defintionFile));

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

    private void PropagateModels()
    {
        foreach (var v in _definitionContext)
        {
            switch (_definitionContext.GetVariable(v).Value)
            {
                case ItemModel item:
                    _customSections.ItemsSection.Elements.Add(item);
                    break;
                case AttributeModel attribute:
                    _customSections.AttributesSection.Elements.Add(attribute);
                    break;
                case MetaModel meta:
                    {
                        foreach (var (key, value) in meta.Properties)
                        {
                            _customSections.MetaSection.Properties[$"{meta.Name}.{key}"] = value;
                        }

                        break;
                    }
            }
        }
    }

    private void ApplyInventory(HoconObject obj, IHasItems model)
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

    private void ApplyNpcs(HoconObject obj, RoomModel model)
    {
        if (!obj.ContainsKey("npcs"))
        {
            return;
        }

        foreach (var character in obj.GetField("npcs").GetArray())
        {
            model.NPCS.Add(new(character.GetString()));
        }
    }

    private void WriteCharacter(string name, HoconObject obj)
    {
        var description = obj.GetField("description").GetString();
        var isNPC = obj.GetField("isNPC").GetString() == "true";

        var model = new CharacterModel(name, description, isNPC);

        ApplyInventory(obj, model);

        _customSections.CharactersSection.Elements.Add(model);
    }

    private void WriteItem(string name, HoconObject obj)
    {

        //   ApplyCode(obj, model.OnInteraction, "on_interaction");

    }

    private void WriteRoom(string name, HoconObject obj)
    {
        var description = obj.GetField("description").GetString();

        var model = new RoomModel(name, description);

        ApplyInventory(obj, model);
        ApplyNpcs(obj, model);
        ApplyExits(obj, model);
        //ApplyCode(obj, model.OnEnter, "on_enter");
        //ApplyCode(obj, model.OnExit, "on_exit");

        _customSections.RoomsSection.Elements.Add(model);
    }

    private void ApplyExits(HoconObject obj, RoomModel model)
    {
        if (!obj.ContainsKey("exits"))
        {
            return;
        }

        foreach (var (direction, value) in obj.GetField("exits").GetObject())
        {
            var exitObj = value.GetObject();
            var name = exitObj.GetField("name").GetString();
            var description = exitObj.GetField("description").GetString();
            var isLocked = GetOptionalFieldValue<bool>(exitObj, "isLocked");

            model.Exits.Add(new ExitModel()
            {
                Direction = Enum.Parse<Direction>(direction, true),
                Name = name,
                Description = description,
                IsLocked = isLocked
            });
        }
    }

    private void WriteRegion(string name, HoconObject obj)
    {
        var description = obj.GetField("description").GetString();
        var rooms = obj.GetField("rooms").GetObject();

        var roomDict = new Dictionary<ModelRef, Position>();
        foreach (var (roomName, roomObj) in rooms)
        {
            var nameRef = new ModelRef(roomName);
            roomDict[nameRef] = Position.Parse(roomObj);
        }

        var model = new RegionModel(name, description, roomDict);

        if (obj.TryGetValue("startRoom", out HoconField value))
        {
            model.StartRoom = value.GetString();
        }

        _customSections.RegionsSection.Elements.Add(model);
    }

    private T GetOptionalFieldValue<T>(HoconObject obj, string fieldName, T defaultValue = default)
        where T : IParsable<T>
    {
        return obj.ContainsKey(fieldName) ? T.Parse(obj.GetField(fieldName).GetString(), CultureInfo.InvariantCulture) : defaultValue;
    }

    public void Close()
    {
        if (IsClosed)
        {
            return;
        }

        _customSections.PopulateSymbolTable(_symbolTable);

        _customSections.Write();

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
            var exceptions = new List<Exception>();
            foreach (var message in diagnostics.Messages)
            {
                exceptions.Add(new Exception(message.Message));
            }
            throw new AggregateException(exceptions);
        }
    }

    public void Dispose()
    {
        Close();
    }
}
