using System.Collections.Generic;
using MessagePack;
using ObjectModel.Evaluation;
using ObjectModel.Referencing;

namespace ObjectModel.Models;

[MessagePackObject(AllowPrivate = true)]
internal class RoomModel : GameObjectModel, IHasItems
{
    [Key(4)]
    public List<ModelRef> Items { get; set; } = [];

    [Key(5)]
    public List<ModelRef> NPCS { get; set; } = [];

    [Key(6)]
    public List<ExitModel> Exits { get; set; } = [];

    [Key(7)]
    public string OnEnter { get; set; } = string.Empty;

    [Key(8)]
    public string OnExit { get; set; } = string.Empty;

    public RoomModel(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
