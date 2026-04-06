using System.Collections.Generic;
using System.Linq;
using NiL.JS.Core;
using ObjectModel.Models;
using ObjectModel.Referencing;

namespace ObjectModel.Parsing;

[CustomCodeFragment]
internal sealed class ItemDefinition : DefinitionCodeNode<ItemModel>
{
    public Dictionary<string, CodeNode> OnInteraction { get; set; } = [];

    public static bool Validate(string code, int position)
    {
        return Parser.Validate(code, "item", position);
    }

    public static CodeNode Parse(ParseInfo state, ref int position)
    {
        if (!ParseHeaderWithName("item", out var name, ref position, state))
        {
            return null;
        }

        var item = new ItemDefinition();
        item.Model.Name = name;

        if (ParseProperties(state, ref position, out var properties))
        {
            AddPropertiesToModel(item.Model, properties);
            item.Model.IsPlayerVisible = GetPropertyValue(properties, "visible", true);
        }

        return item;
    }

    private static void AddPropertiesToModel(GameObjectModel model, Dictionary<string, object> properties)
    {
        model.Description = GetPropertyValue<string>(properties, "description");
        model.Commands = GetPropertyValue(properties, "commands", new List<string>());
        model.Attributes = GetPropertyValue(properties, "attributes", new Dictionary<string, object>())
            .ToDictionary(k => (ModelRef)k.Key, v => (int)v.Value);
    }
}