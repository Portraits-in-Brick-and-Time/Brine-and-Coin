using System.Collections.Generic;

namespace ObjectModel.Evaluation;

public class Scope
{
    private Dictionary<string, IEvaluable> _variables = [];
    public Scope Parent { get; private set; }

    public Scope NewSubScope()
    {
        return new()
        {
            Parent = this
        };
    }

    public IEvaluable GetValue(string name)
    {
        var scope = this;
        while (scope != null)
        {
            if (_variables.TryGetValue(name, out var value))
            {
                return value;
            }

            scope = scope.Parent;
        }

        return null;
    }

    public void AddOrSet(string name, IEvaluable value)
    {
        _variables[name] = value;
    }

    public void AddFunction(IFunction function)
    {
        _variables[function.Name] = function;
    }
}