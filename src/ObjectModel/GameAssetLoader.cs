using System.Collections.Generic;
using System.IO;
using System.Linq;
using NetAF.Assets;
using NetAF.Assets.Attributes;
using NetAF.Assets.Characters;
using NetAF.Assets.Locations;
using NetAF.Commands;
using NetAF.Commands.Persistence;
using NetAF.Conversations;
using NetAF.Conversations.Instructions;
using ObjectModel.Evaluation;
using ObjectModel.IO;
using ObjectModel.Models;
using ObjectModel.Referencing;
using Splat;

namespace ObjectModel;

public class GameAssetLoader
{
    private readonly List<Attribute> _attributes = [];
    private readonly List<Item> _items = [];
    private readonly List<PlayableCharacter> _players = [];
    private readonly List<NonPlayableCharacter> _npcs = [];
    private readonly List<Room> _rooms = [];
    private readonly List<Region> _regions = [];
    private readonly CustomSections customSections;

    private GameAssetLoader(CustomSections customSections)
    {
        this.customSections = customSections;
    }

    public static Overworld LoadFile(out PlayableCharacter[] players)
    {
        const string path = "Assets/core_assets.elf";
        if (!File.Exists(path))
        {
            throw new System.Exception($"Cannot load assets from '{path}'");
        }

        var reader = new GameAssetReader(File.OpenRead(path));
        var loader = new GameAssetLoader(reader.CustomSections);

        loader.Load();
        players = [.. loader._players];

        return loader.BuildWorld();
    }

    private void Load()
    {
        // Build all independent components first
        LoadAttributes();
        LoadItems();
        LoadCharacters();
        LoadFunctions();

        // Then build dependent components
        LoadRooms();
        LoadRegions();
    }

    private void LoadFunctions()
    {
        foreach (var funcDef in customSections.FunctionDefinitionsSection.Elements)
        {
            var evaluator = Locator.Current.GetService<Evaluator>();
            evaluator.RootScope.AddCustomFunction(funcDef.Name, funcDef.Parameters, funcDef.Action);
        }
    }

    private Overworld BuildWorld()
    {
        var worldName = customSections.MetaSection.Properties["world.name"];
        var worldDescription = customSections.MetaSection.Properties["world.description"];

        var overworld = new Overworld(worldName.ToString(), worldDescription.ToString(),
            commands: CreatePersistentCommands());

        foreach (var region in customSections.RegionsSection.Elements)
        {
            overworld.AddRegion(GetRegionByName(region.Name));
        }

        return overworld;
    }

    public static (string folder, string path) GetSaveFileName()
    {
        var folder = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "BrineAndCoin");
        var path = Path.Combine(
            folder,
            "savegame.json"
        );

        return (folder, path);
    }

    private CustomCommand[] CreatePersistentCommands()
    {
        var (folder, path) = GetSaveFileName();

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        return [
            ConvertCommand(new Load(path)),
            ConvertCommand(new Save(path)),
        ];
    }

    private static CustomCommand ConvertCommand(ICommand command)
    {
        return new CustomCommand(command.Help, true, true, (game, args) =>
        {
            return command.Invoke(game);
        });
    }

    private Region GetRegionByName(string name)
    {
        foreach (var region in _regions)
        {
            if (region.Identifier.Name == name)
            {
                return region;
            }
        }

        throw new KeyNotFoundException($"Region '{name}' not found.");
    }

    private void LoadAttributes()
    {
        foreach (var attrModel in customSections.AttributesSection.Elements)
        {
            _attributes.Add(new Attribute(attrModel.Name, attrModel.Description, attrModel.Min, attrModel.Max, attrModel.Visible));
        }
    }

    private void LoadItems()
    {
        foreach (var itemModel in customSections.ItemsSection.Elements)
        {
            var item = new Item(itemModel.Name, itemModel.Description, commands: GetCommands(itemModel),
                interaction: GetInteraction(itemModel.OnInteraction))
            {
                IsPlayerVisible = itemModel.IsPlayerVisible
            };

            ApplyAttributes(item, itemModel);
            
            _items.Add(item);
        }
    }

    private InteractionCallback GetInteraction(List<IEvaluable> onInteraction)
    {
        if (onInteraction.Count == 0)
        {
            return null;
        }

        return new(item =>
        {
            var interaction = EvaluateCode<Interaction>(onInteraction);

            return new Interaction(interaction.Result, item, interaction.Description);
        });
    }

    private void LoadRegions()
    {
        foreach (var regionModel in customSections.RegionsSection.Elements)
        {
            var region = new Region(regionModel.Name, regionModel.Description, commands: GetCommands(regionModel));
            foreach (var (roomRef, (x, y, z)) in regionModel.Rooms)
            {
                region.AddRoom(_rooms.Find(r => r.Identifier.Name == roomRef.Name), x, y, z);
            }

            if (regionModel.StartRoom is not null)
            {
                region.SetStartRoom(_rooms.Find(r => r.Identifier.Name == regionModel.StartRoom?.Name));
            }

            ApplyAttributes(region, regionModel);
            _regions.Add(region);
        }
    }

    private void LoadRooms()
    {
        foreach (var roomModel in customSections.RoomsSection.Elements)
        {
            var room = new Room(roomModel.Name, roomModel.Description,
                commands: GetCommands(roomModel),
                exits: GetExits(roomModel),
                enterCallback: ApplyRoomTransition(roomModel.OnEnter),
                exitCallback: ApplyRoomTransition(roomModel.OnExit)
            );
            ApplyAttributes(room, roomModel);
            AddItems(room, roomModel);
            AddNpcs(room, roomModel);

            _rooms.Add(room);
        }
    }

    private Exit[] GetExits(RoomModel model)
    {
        var exits = new List<Exit>();

        foreach (var exitModel in model.Exits)
        {
            var exit = new Exit(exitModel.Direction, exitModel.IsLocked, new(exitModel.Name),
                new Description(exitModel.Description));
            exits.Add(exit);
        }

        return [.. exits];
    }

    private RoomTransitionCallback ApplyRoomTransition(List<IEvaluable> code)
    {
        if (code.Count == 0)
        {
            return null;
        }

        return new(transition =>
        {
            Evaluator evaluator = Locator.Current.GetService<Evaluator>();
            var scope = evaluator.RootScope.NewSubScope();
            //Todo: add transition to scope when mutliple value types supported
            //scope.AddOrSet("transition", transition);
            return new(EvaluateCode<Reaction>(code, scope), true);
        });
    }

    private object EvaluateCode(List<IEvaluable> code, Scope scope = null)
    {
        Evaluator evaluator = Locator.Current.GetService<Evaluator>();
        return evaluator.Evaluate(code, scope ?? evaluator.RootScope);
    }

    private T EvaluateCode<T>(List<IEvaluable> code, Scope scope = null)
    {
        return (T)EvaluateCode(code, scope ?? Locator.Current.GetService<Evaluator>().RootScope);
    }

    private void AddNpcs(Room target, RoomModel model)
    {
        foreach (var characterRef in model.NPCS)
        {
            var character = GetByRef(characterRef, _npcs);
            target.AddCharacter(character);
        }
    }

    private void LoadCharacters()
    {
        foreach (var charModel in customSections.CharactersSection.Elements)
        {
            var commands = GetCommands(charModel);
            var items = GetItems(charModel).ToArray();

            Character character;
            if (charModel.IsNPC)
            {
                var c = new Conversation(
                    [.. items.Select(item => new Paragraph(item.Identifier.Name, game => game.Player.AddItem(item)))]
                );
                character = new NonPlayableCharacter(charModel.Name, charModel.Description, commands: commands, conversation: c);

                _npcs.Add((NonPlayableCharacter)character);
            }
            else
            {
                character = new PlayableCharacter(charModel.Name, charModel.Description, commands: commands);
                _players.Add((PlayableCharacter)character);
            }

            ApplyAttributes(character, charModel);
            foreach (var item in items)
            {
                character.AddItem(item);
            }
        }
    }

    private void AddItems(IItemContainer target, IItemModel model)
    {
        foreach (var item in GetItems(model))
        {
            target.AddItem(item);
        }
    }

    private void ApplyAttributes(IExaminable target, GameObjectModel model)
    {
        foreach (var (name, value) in model.Attributes)
        {
            target.Attributes.Add(GetAttributeByRef(name), value);
        }
    }

    private static CustomCommand[] GetCommands(GameObjectModel model)
    {
        var cmds = new List<CustomCommand>();
        foreach (var @ref in model.Commands)
        {
            if (CommandStore.TryGet(@ref, out var cmd))
            {
                cmds.Add(cmd);
            }
            else
            {
                throw new KeyNotFoundException($"Command '{@ref}' not found.");
            }
        }

        return [.. cmds];
    }

    private IEnumerable<Item> GetItems(IItemModel model)
    {
        foreach (var itemRef in model.Items)
        {
            var item = GetByRef(itemRef, _items);
            yield return item;
        }
    }

    Attribute GetAttributeByRef(ModelRef @ref)
    {
        foreach (var attribute in _attributes)
        {
            if (attribute.Name == @ref.Name)
            {
                return attribute;
            }
        }

        throw new KeyNotFoundException($"Attribute '{@ref}' not found.");
    }

    T GetByRef<T>(ModelRef @ref, IList<T> collection)
        where T : IExaminable
    {
        foreach (var element in collection)
        {
            if (element.Identifier.Name == @ref.Name)
            {
                return element;
            }
        }

        throw new KeyNotFoundException($"Item '{@ref}' not found.");
    }
}