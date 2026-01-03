using System.Collections.Generic;
using MessagePack;
using NetAF.Assets;
using ObjectModel.Sections;

namespace ObjectModel.Models;

public abstract class GameObject
{
    [Key(0)]
    public string Name { get; set; }

    [Key(1)]
    public string Description { get; set; }

    [Key(2)]
    public Dictionary<NamedRef, int> Attributes { get; set; } = [];

    public abstract IExaminable Instanciate(CustomSections customSections);

    public void InstanciateAttributesTo(IExaminable target, AttributesSection attributesSection)
    {
        foreach (var (key, value) in Attributes)
        {
            target.Attributes.Add(attributesSection.GetAttributeByName(key.Name), value);
        }
    }
}
