using System.Collections.Generic;
using NiL.JS.BaseLibrary;
using NiL.JS.Core;
using ObjectModel.Models;

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

        // Parse properties until closing brace
        int start = position;
        while (position < state.Code.Length)
        {
            SkipWhitespace(state.Code, ref position);

            // Check for closing brace
            if (Parser.Validate(state.Code, "}", ref position))
            {
                break;
            }

            // Parse property name
            start = position;
            while (position < state.Code.Length && (char.IsLetterOrDigit(state.Code[position]) || state.Code[position] == '_'))
            {
                position++;
            }

            if (start == position)
            {
                throw new JSException(new SyntaxError("Expected property name at " + CodeCoordinates.FromTextPosition(state.Code, position, 3)));
            }

            string propertyName = state.Code[start..position];
            SkipWhitespace(state.Code, ref position);

            if (ParsePropertyPrefix(state, ref position, propertyName, "description"))
            {
                if (ParseString(state, ref position, out var description))
                {
                    item.Model.Description = description;
                    continue;
                }
            }
            else if (ParsePropertyPrefix(state, ref position, propertyName, "visible"))
            {
                if (ParseBoolean(state, ref position, out var isVisible))
                {
                    item.Model.IsPlayerVisible = isVisible;
                    continue;
                }
            }
            else if (propertyName == "attributes")
            {
                item.Model.Attributes = ParseAttributes(state, ref position);
            }

            else if (propertyName == "commands")
            {
                item.Model.Commands = ParseCommands(state, ref position);
            }
            else if (propertyName == "on_interaction")
            {
                ParseOnInteraction(state, ref position, item);
            }
            else
            {
                // Skip unknown properties
                SkipProperty(state.Code, ref position);
            }

            SkipWhitespace(state.Code, ref position);

            // Handle optional comma or newline between properties
            if (position < state.Code.Length && state.Code[position] == ',')
            {
                position++;
            }
        }

        return item;
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
            SkipNestedStructure(state.Code, ref position);
        }
    }
}