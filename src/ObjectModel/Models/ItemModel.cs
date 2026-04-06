using System.Collections.Generic;
using MessagePack;
using ObjectModel.Evaluation;

namespace ObjectModel.Models;

[MessagePackObject(AllowPrivate = true)]
internal class ItemModel : GameObjectModel
{
    [Key(4)]
    public bool IsPlayerVisible { get; set; }

    [Key(5)]
    public string OnInteraction { get; set; } = string.Empty;

    public ItemModel(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public ItemModel()
    {
        
    }
}