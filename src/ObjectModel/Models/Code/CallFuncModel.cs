using System.Collections.Generic;
using Hocon;
using MessagePack;
using ObjectModel.Evaluation;

namespace ObjectModel.Models.Code;

[MessagePackObject(AllowPrivate = true)]
internal class CallFuncModel(string name, Dictionary<string, IEvaluable> parameters) : IEvaluable
{
    [Key(0)]
    public string Name { get; set; } = name;

    [Key(1)]
    public Dictionary<string, IEvaluable> Parameters { get; set; } = parameters;

    public object Evaluate(Evaluator evaluator, Scope scope)
    {
        var evaluatable = scope.GetValue(Name);
        if (evaluatable is not IFunction func)
        {
            throw new System.InvalidOperationException($"Function '{Name}' not found.");
        }

        var evaluatedParams = new List<object>();

        foreach (var param in Parameters)
        {
            evaluatedParams.Add(param.Value.Evaluate(evaluator, scope));
        }

        return func.Invoke([.. evaluatedParams]);
    }

    public static IEvaluable FromObject(KeyValuePair<string, HoconField> rootObj)
    {
        var funcName = rootObj.Key;
        var paramList = new Dictionary<string, IEvaluable>();

        var paramArray = rootObj.Value.GetObject();
        foreach (var param in paramArray)
        {
            paramList.Add(param.Key, new ValueModel(param.Value.GetString()));
        }

        return new CallFuncModel(funcName, paramList);
    }
}