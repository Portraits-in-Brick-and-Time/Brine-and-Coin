using System.Collections.Generic;
using MessagePack;

namespace ObjectModel.Models.Code;

[MessagePackObject(AllowPrivate = true)]
internal class FuncDefModel : GameObjectModel
{
    [Key(4)]
    public List<string> Parameters { get; set; } = [];
}