using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NiL.JS.BaseLibrary;
using NiL.JS.Core;
using NiL.JS.Expressions;
using NiL.JS.Statements;
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

    protected static bool ParseProperty(ParseInfo state, ref int position, out string name, out object value)
    {
        _ = ParsePropertyName(state, ref position, out name);

        SkipWhitespace(state.Code, ref position);

        // nested block
        if (Parser.Validate(state.Code, "{", ref position))
        {
            if (name.StartsWith("on_"))
            {
                value = ParseCodeBlock(state, ref position);

                return true;
            }

            if (ParseProperties(state, ref position, out var nestedProperties))
            {
                value = nestedProperties;
                return true;
            }
        }

        // Parse simple property
        if (!Parser.Validate(state.Code, "=", ref position))
        {
            throw new JSException(new SyntaxError($"Expected \"=\" at {CodeCoordinates.FromTextPosition(state.Code, position, 6)}"));
        }
        SkipWhitespace(state.Code, ref position);

        value = UnMarshal(ExpressionTree.Parse(state, ref position).Evaluate(Context.CurrentGlobalContext));
        return true;
    }

    private static bool ParsePropertyName(ParseInfo state, ref int position, out string name)
    {
        int start = position;
        while (position < state.Code.Length && (char.IsLetterOrDigit(state.Code[position]) || state.Code[position] == '_'))
        {
            position++;
        }

        if (start == position)
        {
            throw new JSException(new SyntaxError("Expected property name at " + CodeCoordinates.FromTextPosition(state.Code, position, 3)));
        }

        name = state.Code[start..position];
        return true;
    }

    private static string ParseCodeBlock(ParseInfo state, ref int position)
    {
        position--; // step back to include the opening brace in the code block parsing
        var parseMethod = typeof(CodeBlock).GetMethod(
            "Parse",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public,
            null,
            [typeof(ParseInfo), typeof(int).MakeByRefType()],
            null);

        var args = new object[] { state, position };
        return (parseMethod.Invoke(null, args) as CodeBlock).Code;
    }

    private static object UnMarshal(JSValue value)
    {
        if (value is NiL.JS.BaseLibrary.Array arr)
        {
            return arr.Select(k => UnMarshal(k.Value)).ToList();
        }

        return value.Value;
    }

    protected static bool ParseProperties(ParseInfo state, ref int position, out Dictionary<string, object> properties)
    {
        properties = [];

        // Parse properties until closing brace
        while (position < state.Code.Length)
        {
            SkipWhitespace(state.Code, ref position);

            // Check for closing brace
            if (Parser.Validate(state.Code, "}", ref position))
            {
                break;
            }

            if (ParseProperty(state, ref position, out var propertyName, out var propertyValue))
            {
                properties[propertyName] = propertyValue;
            }

            SkipWhitespace(state.Code, ref position);

            // Handle optional comma
            if (position < state.Code.Length && state.Code[position] == ',')
            {
                position++;
            }
        }

        return true;
    }

    protected static T GetPropertyValue<T>(Dictionary<string, object> dict, string path, T defaultValue = default)
    {
        var parts = path.Split('.');
        object current = dict;

        foreach (var part in parts)
        {
            if (current is Dictionary<string, object> currentDict && currentDict.TryGetValue(part, out var value))
            {
                current = value;
            }
            else
            {
                return defaultValue;
            }
        }

        return ConvertToType<T>(current, defaultValue);
    }

    private static T ConvertToType<T>(object value, T defaultValue)
    {
        if (value == null)
            return defaultValue;

        var targetType = typeof(T);

        // Handle List types
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
        {
            var elementType = targetType.GetGenericArguments()[0];
            if (value is IEnumerable<object> list)
            {
                try
                {
                    var convertedList = (IList)Activator.CreateInstance(targetType);
                    foreach (var item in list)
                    {
                        var convertedItem = Convert.ChangeType(item, elementType);
                        convertedList.Add(convertedItem);
                    }
                    return (T)convertedList;
                }
                catch
                {
                    return defaultValue;
                }
            }
        }

        // Handle simple types
        try
        {
            return (T)Convert.ChangeType(value, targetType);
        }
        catch
        {
            return defaultValue;
        }
    }

    protected static void AddPropertiesToModel(GameObjectModel model, Dictionary<string, object> properties)
    {
        model.Description = GetPropertyValue<string>(properties, "description");
        model.Commands = GetPropertyValue(properties, "commands", new List<string>());
        model.Attributes = GetPropertyValue(properties, "attributes", new Dictionary<string, object>())
            .ToDictionary(k => (ModelRef)k.Key, v => (int)v.Value);
    }
}
