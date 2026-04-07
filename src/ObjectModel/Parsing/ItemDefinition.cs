using NiL.JS.Core;
using ObjectModel.Models;

namespace ObjectModel.Parsing;

[CustomCodeFragment]
internal sealed class ItemDefinition : DefinitionCodeNode<ItemModel>
{
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
            item.Model.OnInteraction = GetPropertyValue<string>(properties, "on_interaction");
        }

        return item;
    }
}
