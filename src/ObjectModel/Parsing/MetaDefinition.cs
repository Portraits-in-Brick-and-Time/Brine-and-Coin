using NiL.JS.Core;
using ObjectModel.Models;

namespace ObjectModel.Parsing;

[CustomCodeFragment]
internal sealed class MetaDefinition : DefinitionCodeNode<MetaModel>
{
    public static bool Validate(string code, int position)
    {
        return Parser.Validate(code, "meta", position);
    }

    public static CodeNode Parse(ParseInfo state, ref int position)
    {
        if (!ParseHeaderWithName("meta", out var name, ref position, state))
        {
            return null;
        }

        var meta = new MetaDefinition();
        meta.Model.Name = name;

        if (ParseProperties(state, ref position, out var properties))
        {
            meta.Model.Properties = properties;
        }

        return meta;
    }
}
