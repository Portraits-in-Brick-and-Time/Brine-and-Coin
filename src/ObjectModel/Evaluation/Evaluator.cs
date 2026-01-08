using System.Collections.Generic;
using ObjectModel.Models.Code;

namespace ObjectModel.Evaluation;

public class Evaluator
{
    public Scope RootScope = new();
    public T Evaluate<T>(List<IEvaluable> code, Scope scope)
    {
        object result = null;
        for (int i = 0; i < code.Count; i++)
        {
            result = code[i].Evaluate(this, scope);
        }

        return (T)result;
    }
}
