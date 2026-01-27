using System.Collections.Generic;
using Hocon;
using MessagePack;
using ObjectModel.Evaluation;

namespace ObjectModel.Models.Code;

[MessagePackObject(AllowPrivate = true)]
internal class ValueModel(string value) : IEvaluable
{
    [Key(0)]
    public string Value { get; set; } = value;

    public ValueModel() : this(null)
    {
    }

    public object Evaluate(Evaluator evaluator, Scope scope)
    {
        return Value;
    }

    public static IEvaluable FromObject(KeyValuePair<string, HoconField> rootObj)
    {
        return new ValueModel() { Value = rootObj.Value.GetString() };
    }
}
