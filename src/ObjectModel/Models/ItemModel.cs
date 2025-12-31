using MessagePack;
using NetAF.Assets;

namespace ObjectModel.Models;

[MessagePackObject]
public class ItemModel : GameObject
{
    public ItemModel(string name, string description)
    {
        Name = name;
        Description = description;
    }
    public ItemModel()
    {

    }

    public override IExaminable Instanciate()
    {
        return new Item(Name, Description);
    }
}