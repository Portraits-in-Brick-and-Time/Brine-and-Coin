using System.Collections.Generic;
using System.Linq;

namespace ObjectModel.Evaluation;

public interface IFunction : IEvaluable
{
    string Name { get; }
    List<string> Parameters { get; }

    object Invoke(params object[] parameters);
}


public abstract class Function : IFunction
{
    public abstract string Name { get; }
    public abstract List<string> Parameters { get; }

    public object Evaluate(Evaluator evaluator, Scope scope)
    {
        var arguments = Parameters.Select(p => scope.GetValue(p)).ToArray();
        return Invoke(arguments);
    }

    public abstract object Invoke(params object[] parameters);
}

public abstract class Function<T1, T2, TResult> : Function
{
    public override object Invoke(params object[] parameters)
    {
        if (parameters.Length != 2)
            throw new System.ArgumentException($"Function {Name} expects 2 parameters.");

        return Invoke((T1)parameters[0], (T2)parameters[1]);
    }

    public abstract TResult Invoke(T1 param1, T2 param2);
}

public abstract class Function<T1, TResult> : Function
{
    public override object Invoke(params object[] parameters)
    {
        if (parameters.Length != 2)
            throw new System.ArgumentException($"Function {Name} expects 2 parameters.");

        return Invoke((T1)parameters[0]);
    }

    public abstract TResult Invoke(T1 param1);
}