using System.Collections.Generic;
using MessagePack;
using ObjectModel.Referencing;

namespace ObjectModel.Models;

[MessagePackObject(AllowPrivate = true)]
internal class CharacterModel : GameObjectModel, IItemModel
{
    public CharacterModel(string name, string description, bool isNPC)
    {
        Name = name;
        Description = description;
        IsNPC = isNPC;
    }
    public CharacterModel()
    {
    }

    [Key(4)]
    public bool IsNPC { get; set; }

    [Key(5)]
    public List<ModelRef> Items { get; set; } = [];

}