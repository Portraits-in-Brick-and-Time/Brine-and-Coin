using System.Collections.Generic;
using Hocon;
using MessagePack;

namespace ObjectModel.Models.Code;

[Union(0, typeof(ReactionModel))]
internal interface IEvaluable
{
    object Evaluate();
    static virtual IEvaluable FromObject(KeyValuePair<string, HoconField> rootObj)
    {
        return null;
    }
}