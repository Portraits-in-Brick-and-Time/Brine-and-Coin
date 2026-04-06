using System.Collections.Generic;

namespace ObjectModel.Models;

// dont need to be serialized, just a container for meta information about the game
internal class MetaModel : GameObjectModel
{
    public Dictionary<string, object> Properties { get; set; } = [];
}
