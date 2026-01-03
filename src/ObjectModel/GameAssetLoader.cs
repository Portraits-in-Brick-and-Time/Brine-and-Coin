using System.Collections.Generic;
using System.IO;
using NetAF.Assets;
using NetAF.Assets.Attributes;
using NetAF.Assets.Characters;
using NetAF.Assets.Locations;
using ObjectModel.IO;
using ObjectModel.Models;

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

    public static Overworld LoadFile()
    {
        const string path = "Assets/core_assets.elf";
        if (!File.Exists(path))
        {
            throw new System.Exception($"Cannot load assets from '{path}'");
        }

        var reader = new GameAssetReader(File.OpenRead(path));
        var loader = new GameAssetLoader(reader.CustomSections);

        loader.Load();

        return loader.BuildWorld();
    }

    private void Load()
    {
        // Build all independent components first
        LoadAttributes();
        LoadItems();
        LoadCharacters();

        // Then build dependent components
        LoadRooms();
        LoadRegions();
    }

    private Overworld BuildWorld()
    {
        var worldName = customSections.MetaSection.Properties["world.name"];
        var worldDescription = customSections.MetaSection.Properties["world.description"];

        var overworld = new Overworld(worldName.ToString(), worldDescription.ToString());

        foreach (var region in customSections.RegionsSection.Regions)
        {
            overworld.AddRegion(GetRegionByName(region.Name));
        }

        return overworld;
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
        foreach (var attrModel in customSections.AttributesSection.Attributes)
        {
            _attributes.Add(new Attribute(attrModel.Name, attrModel.Description, attrModel.Min, attrModel.Max, attrModel.Visible));
        }
    }

    private void LoadItems()
    {
        foreach (var itemModel in customSections.ItemsSection.Items)
        {
            var item = new Item(itemModel.Name, itemModel.Description);
            ApplyAttributes(item, itemModel);
            _items.Add(item);
        }
    }

    private void LoadRegions()
    {
        foreach (var regionModel in customSections.RegionsSection.Regions)
        {
            var region = new Region(regionModel.Name, regionModel.Description);
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
        foreach (var roomModel in customSections.RoomsSection.Rooms)
        {
            var room = new Room(roomModel.Name, roomModel.Description);
            ApplyAttributes(room, roomModel);
            _rooms.Add(room);
        }
    }

    private void LoadCharacters()
    {
        foreach (var charModel in customSections.CharactersSection.Characters)
        {
            Character character;
            if (charModel.IsNPC)
            {
                character = new NonPlayableCharacter(charModel.Name, charModel.Description);
                _npcs.Add((NonPlayableCharacter)character);
            }
            else
            {
                character = new PlayableCharacter(charModel.Name, charModel.Description);
                _players.Add((PlayableCharacter)character);
            }

            ApplyAttributes(character, charModel);
        }
    }

    private void ApplyAttributes(IExaminable target, GameObjectModel model)
    {
        foreach (var (name, value) in model.Attributes)
        {
            target.Attributes.Add(GetAttributeByRef(name), value);
        }
    }

    private Attribute GetAttributeByRef(NamedRef name)
    {
        foreach (var attribute in _attributes)
        {
            if (attribute.Name == name.Name)
            {
                return attribute;
            }
        }

        throw new KeyNotFoundException($"Attribute '{name}' not found.");
    }

}