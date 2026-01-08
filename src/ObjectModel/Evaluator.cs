using System.Collections.Generic;
using ObjectModel.Models.Code;

namespace ObjectModel;

internal class Evaluator
{
    public T Evaluate<T>(List<IEvaluable> code)
    {
        object result = null;
        for (int i = 0; i < code.Count; i++)
        {
            result = code[i].Evaluate();
        }

        return (T)result;
    }
}