using NiL.JS.Core;

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
