using System.Collections.Generic;
using Hocon;
using MessagePack;
using ObjectModel.Models.Code;

namespace ObjectModel.Evaluation;

[Union(0, typeof(ValueModel))]
[Union(1, typeof(VariableDefinitonModel))]
[Union(2, typeof(CallFuncModel))]
public interface IEvaluable
{
    object Evaluate(Evaluator evaluator, Scope scope);
    static virtual IEvaluable FromObject(KeyValuePair<string, HoconField> rootObj)
    {
        return null;
    }
}