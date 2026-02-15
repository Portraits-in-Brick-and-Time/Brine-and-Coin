using System.Collections.Generic;
using System.Linq;

namespace ObjectModel.Evaluation;

internal class CustomFunction(string name, string[] parameters, List<IEvaluable> code) : IEvaluable
{
    public string Name => name;

    public string[] Parameters => parameters;

    public object Evaluate(Evaluator evaluator, Scope scope)
    {
        var arguments = Parameters.Select(scope.GetValue).ToArray();
        var subScope = scope.NewSubScope();

        for (int i = 0; i < parameters.Length; i++)
        {
            subScope.AddOrSet(parameters[i], arguments[i]);
        }

        return evaluator.Evaluate(code, subScope);
    }

    public override string ToString()
    {
        return $"{Name}({string.Join(',', Parameters)})";
    }
}