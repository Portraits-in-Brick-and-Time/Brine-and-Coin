using System.Collections.Generic;
using System.Linq;
using NiL.JS.BaseLibrary;
using NiL.JS.Core;
using ObjectModel.Models;
using ObjectModel.Referencing;

namespace ObjectModel.Parsing;

[CustomCodeFragment]
internal sealed class AttributeDefinition : DefinitionCodeNode<AttributeModel>
{
    public static bool Validate(string code, int position)
    {
        return Parser.Validate(code, "attribute", position);
    }

    public static CodeNode Parse(ParseInfo state, ref int position)
    {
        if (!ParseHeaderWithName("attribute", out var name, ref position, state))
        {
            return null;
        }

        var attribute = new AttributeDefinition();
        attribute.Model.Name = name;

        if (ParseProperties(state, ref position, out var properties))
        {
            attribute.Model.Description = GetPropertyValue<string>(properties, "description");
            attribute.Model.Min = GetPropertyValue(properties, "min", int.MinValue);
            attribute.Model.Max = GetPropertyValue(properties, "max", int.MaxValue);
            attribute.Model.Visible = GetPropertyValue(properties, "visible", true);
        }

        return attribute;
    }
}

[CustomCodeFragment]
internal sealed class ItemDefinition : DefinitionCodeNode<ItemModel>
{
    public Dictionary<string, CodeNode> OnInteraction { get; set; } = [];

    public static bool Validate(string code, int position)
    {
        return Parser.Validate(code, "item", position);
    }

    private Dictionary<string, int> ParseAttributes(ParseInfo state, ref int position)
    {
        var attributes = new Dictionary<string, int>();

        

        return attributes;
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

    private static void ParseOnInteraction(ParseInfo state, ref int position, ItemDefinition item)
    {
        SkipWhitespace(state.Code, ref position);
        if (!Parser.Validate(state.Code, "{", ref position))
        {
            throw new JSException(new SyntaxError("Expected \"{\" for on_interaction at " + CodeCoordinates.FromTextPosition(state.Code, position, 15)));
        }

        while (position < state.Code.Length)
        {
            SkipWhitespace(state.Code, ref position);

            if (Parser.Validate(state.Code, "}", ref position))
            {
                break;
            }

            // Parse nested property name
            var start = position;
            while (position < state.Code.Length && (char.IsLetterOrDigit(state.Code[position]) || state.Code[position] == '_'))
            {
                position++;
            }

            if (start == position)
            {
                throw new JSException(new SyntaxError("Expected property name in on_interaction at " + CodeCoordinates.FromTextPosition(state.Code, position, 16)));
            }

            string propName = state.Code[start..position];
            SkipWhitespace(state.Code, ref position);

            // Skip the nested structure (simplified - just skip until next property or closing brace)
            
        }
    }
}