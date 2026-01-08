using System.Collections.Generic;
using Hocon;
using MessagePack;
using ObjectModel.Evaluation;

namespace ObjectModel.Models.Code;

[MessagePackObject(AllowPrivate = true)]
internal class VariableDefinitonModel : IEvaluable
{
    [Key(0)]
    public string Name { get; set; }

    [Key(1)]
    public IEvaluable Value { get; set; }

    public object Evaluate(Evaluator evaluator, Scope scope)
    {
        var value = Value.Evaluate(evaluator, scope);

        scope.AddOrSet(Name, (IEvaluable)value);

        return value;
    }

    public static IEvaluable FromObject(KeyValuePair<string, HoconField> rootObj)
    {
        var value = rootObj.Value.GetString();

        return new VariableDefinitonModel
        {
            Name = rootObj.Key,
            Value = new ValueModel(value)
        };
    }
}
