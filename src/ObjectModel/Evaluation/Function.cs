using System;
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

    protected T MarshalParameter<T>(object parameter)
    {
        var type = typeof(T);
        if (parameter is string s)
        {
            if (type.IsEnum)
            {
                return (T)Enum.Parse(type, s, ignoreCase: true);
            }

            if (typeof(T).IsAssignableTo(typeof(IParsable<>).MakeGenericType(typeof(T))))
            {
                var method = typeof(T).GetMethod("Parse", [typeof(string)]);

                if (method != null)
                {
                    return (T)method.Invoke(null, [s]);
                }
            }
        }

        return (T)parameter;
    }

    public abstract object Invoke(params object[] parameters);
}

public abstract class Function<T1, T2, T3, TResult> : Function
{
    public override object Invoke(params object[] parameters)
    {
        if (parameters.Length != 3)
            throw new ArgumentException($"Function {Name} expects 3 parameters.");

        return Invoke(MarshalParameter<T1>(parameters[0]), MarshalParameter<T2>(parameters[1]),
            MarshalParameter<T3>(parameters[2]));
    }

    public abstract TResult Invoke(T1 param1, T2 param2, T3 param3);
}

public abstract class Function<T1, T2, TResult> : Function
{
    public override object Invoke(params object[] parameters)
    {
        if (parameters.Length != 2)
            throw new ArgumentException($"Function {Name} expects 2 parameters.");

        return Invoke(MarshalParameter<T1>(parameters[0]), MarshalParameter<T2>(parameters[1]));
    }

    public abstract TResult Invoke(T1 param1, T2 param2);
}

public abstract class Function<T1, TResult> : Function
{
    public override object Invoke(params object[] parameters)
    {
        if (parameters.Length != 1)
            throw new ArgumentException($"Function {Name} expects 1 parameter.");

        return Invoke(MarshalParameter<T1>(parameters[0]));
    }

    public abstract TResult Invoke(T1 param1);
}