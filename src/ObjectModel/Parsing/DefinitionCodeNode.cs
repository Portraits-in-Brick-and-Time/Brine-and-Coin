using System.Collections.Generic;
using NiL.JS.BaseLibrary;
using NiL.JS.Core;
using ObjectModel.Models;
using ObjectModel.Referencing;

namespace ObjectModel.Parsing;

abstract class DefinitionCodeNode<TModel> : CodeNode
    where TModel : GameObjectModel, new()
{
    public TModel Model { get; set; } = new();

    public override JSValue Evaluate(Context context)
    {
        var obj = context.GlobalContext.ProxyValue(Model);

        context.DefineVariable(Model.Name).Assign(obj);
        return obj;
    }

    public override void Decompose(ref CodeNode self)
    {

    }

    protected static void SkipWhitespace(string code, ref int position)
    {
        while (position < code.Length && (char.IsWhiteSpace(code, position) || code[position] == '\n' || code[position] == '\r'))
            position++;
    }

    protected static bool ParseBoolean(ParseInfo state, ref int position, out bool value)
    {
        if (Parser.Validate(state.Code, "true", ref position))
        {
            value = true;
            return true;
        }
        else if (Parser.Validate(state.Code, "false", ref position))
        {
            value = false;
            return true;
        }

        throw new JSException(new SyntaxError("Expected boolean at " + CodeCoordinates.FromTextPosition(state.Code, position, 7)));
    }

    protected static bool ParseString(ParseInfo state, ref int position, out string value)
    {
        int start = position;
        if (Parser.ValidateString(state.Code, ref position, false))
        {
            value = state.Code[(start + 1)..(position - 1)];
            return true;
        }

        throw new JSException(new SyntaxError($"Expected string value at {CodeCoordinates.FromTextPosition(state.Code, position, 5)}"));
    }

    protected static bool ParseHeaderWithName(string defintionName, out string parsedName, ref int position, ParseInfo state)
    {
        if (!Parser.Validate(state.Code, defintionName, ref position))
        {
            parsedName = null;
            return false;
        }

        SkipWhitespace(state.Code, ref position);

        var start = position;
        if (Parser.ValidateString(state.Code, ref position, false))
        {
            parsedName = state.Code[(start + 1)..(position - 1)];
        }
        else
        {
            throw new JSException(new SyntaxError("Expected name at " + CodeCoordinates.FromTextPosition(state.Code, position, 1)));
        }

        SkipWhitespace(state.Code, ref position);
        if (!Parser.Validate(state.Code, "{", ref position))
        {
            throw new JSException(new SyntaxError("Expected \"{\" at " + CodeCoordinates.FromTextPosition(state.Code, position, 2)));
        }

        return true;
    }

    protected static bool ParsePropertyPrefix(ParseInfo state, ref int position, string name, string expectedName)
    {
        if (name != expectedName)
        {
            return false;
        }

        if (!Parser.Validate(state.Code, "=", ref position))
        {
            throw new JSException(new SyntaxError($"Expected \"=\" at {CodeCoordinates.FromTextPosition(state.Code, position, 6)}"));
        }
        SkipWhitespace(state.Code, ref position);

        return true;
    }

    protected static Dictionary<ModelRef, int> ParseAttributes(ParseInfo state, ref int position)
    {
        var attributes = new Dictionary<ModelRef, int>();
        SkipWhitespace(state.Code, ref position);
        if (!Parser.Validate(state.Code, "{", ref position))
        {
            throw new JSException(new SyntaxError("Expected \"{\" for attributes at " + CodeCoordinates.FromTextPosition(state.Code, position, 8)));
        }

        while (position < state.Code.Length)
        {
            SkipWhitespace(state.Code, ref position);

            if (Parser.Validate(state.Code, "}", ref position))
            {
                break;
            }

            // Parse attribute name
            var start = position;
            while (position < state.Code.Length && (char.IsLetterOrDigit(state.Code[position]) || state.Code[position] == '_'))
            {
                position++;
            }

            if (start == position)
            {
                throw new JSException(new SyntaxError("Expected attribute name at " + CodeCoordinates.FromTextPosition(state.Code, position, 9)));
            }

            string attrName = state.Code[start..position];
            SkipWhitespace(state.Code, ref position);

            if (!Parser.Validate(state.Code, "=", ref position))
            {
                throw new JSException(new SyntaxError("Expected \"=\" after attribute name at " + CodeCoordinates.FromTextPosition(state.Code, position, 10)));
            }

            SkipWhitespace(state.Code, ref position);

            // Parse attribute value (integer)
            start = position;
            if (state.Code[position] == '-')
            {
                position++;
            }
            while (position < state.Code.Length && char.IsDigit(state.Code[position]))
            {
                position++;
            }

            if (start == position || (start + 1 == position && state.Code[start] == '-'))
            {
                throw new JSException(new SyntaxError("Expected integer value for attribute at " + CodeCoordinates.FromTextPosition(state.Code, position, 11)));
            }

            if (int.TryParse(state.Code[start..position], out var value))
            {
                attributes[attrName] = value;
            }

            SkipWhitespace(state.Code, ref position);
            if (position < state.Code.Length && state.Code[position] == ',')
            {
                position++;
            }
        }

        return attributes;
    }

    protected static List<string> ParseCommands(ParseInfo state, ref int position)
    {
        var commands = new List<string>();

        SkipWhitespace(state.Code, ref position);
        if (!Parser.Validate(state.Code, "=", ref position))
        {
            throw new JSException(new SyntaxError("Expected \"=\" after commands at " + CodeCoordinates.FromTextPosition(state.Code, position, 12)));
        }

        SkipWhitespace(state.Code, ref position);
        if (!Parser.Validate(state.Code, "[", ref position))
        {
            throw new JSException(new SyntaxError("Expected \"[\" for commands array at " + CodeCoordinates.FromTextPosition(state.Code, position, 13)));
        }

        while (position < state.Code.Length)
        {
            SkipWhitespace(state.Code, ref position);

            if (Parser.Validate(state.Code, "]", ref position))
            {
                break;
            }

            var start = position;
            if (Parser.ValidateString(state.Code, ref position, false))
            {
                commands.Add(state.Code[start..position]);
            }
            else
            {
                throw new JSException(new SyntaxError("Expected string in commands array at " + CodeCoordinates.FromTextPosition(state.Code, position, 14)));
            }

            SkipWhitespace(state.Code, ref position);
            if (position < state.Code.Length && state.Code[position] == ',')
            {
                position++;
            }
        }

        return commands;
    }


    protected static void SkipProperty(string code, ref int position)
    {
        SkipWhitespace(code, ref position);
        if (position < code.Length && code[position] == '=')
        {
            position++;
            SkipWhitespace(code, ref position);
            if (position < code.Length && code[position] == '"')
            {
                Parser.ValidateString(code, ref position, false);
            }
            else if (position < code.Length && code[position] == '[')
            {
                SkipArray(code, ref position);
            }
            else if (position < code.Length && code[position] == '{')
            {
                SkipNestedStructure(code, ref position);
            }
            else
            {
                // Skip until whitespace
                while (position < code.Length && !char.IsWhiteSpace(code[position]))
                {
                    position++;
                }
            }
        }
        else if (position < code.Length && code[position] == '{')
        {
            SkipNestedStructure(code, ref position);
        }
    }

    private static void SkipArray(string code, ref int position)
    {
        if (!Parser.Validate(code, "[", ref position))
            return;

        int bracketCount = 1;
        while (position < code.Length && bracketCount > 0)
        {
            if (code[position] == '[')
                bracketCount++;
            else if (code[position] == ']')
                bracketCount--;
            position++;
        }
    }

    protected static void SkipNestedStructure(string code, ref int position)
    {
        if (!Parser.Validate(code, "{", ref position))
            return;

        int braceCount = 1;
        while (position < code.Length && braceCount > 0)
        {
            if (code[position] == '{')
                braceCount++;
            else if (code[position] == '}')
                braceCount--;
            position++;
        }
    }
}
