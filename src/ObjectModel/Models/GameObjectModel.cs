using System.Collections.Generic;
using MessagePack;

namespace ObjectModel.Models;

internal abstract class GameObjectModel
{
    [Key(0)]
    public string Name { get; set; }

    [Key(1)]
    public string Description { get; set; }

    [Key(2)]
    public Dictionary<ModelRef, int> Attributes { get; set; } = [];

    [Key(3)]
    public List<ModelRef> Commands { get; set; } = [];
}
