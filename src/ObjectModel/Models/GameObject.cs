using MessagePack;
using NetAF.Assets;
using ObjectModel.Sections;

namespace ObjectModel.Models;

[Union(0, typeof(Models.CharacterModel))]
[MessagePackObject]
public abstract class GameObject
{
    [Key(0)]
    public string Name { get; set; }

    [Key(1)]
    public Dictionary<IndexedRef, int> Attributes { get; set; } = [];

    public abstract IExaminable Instanciate();

    public void InstanciateAttributesTo(IExaminable target, AttributesSection attributesSection)
    {
        foreach (var (key, value) in Attributes)
        {
            target.Attributes.Add(attributesSection.Attributes[key.Index].Name, value);
        }
    }
}
