using System;
using System.Collections.Generic;
using Hocon;
using MessagePack;
using NetAF.Commands;
using ObjectModel.Evaluation;

namespace ObjectModel.Models.Code;

[MessagePackObject(AllowPrivate = true)]
internal class ReactionModel : IEvaluable
{
    [Key(0)]
    public ReactionResult Type { get; set; }

    [Key(1)]
    public string Description { get; set; }

    public static IEvaluable FromObject(KeyValuePair<string, HoconField> rootObj)
    {
        HoconObject hoconObject = rootObj.Value.GetObject();

        return new ReactionModel
        {
            Type = Enum.Parse<ReactionResult>(hoconObject.GetField("type").GetString(), true),
            Description = hoconObject.GetField("description").GetString()
        };
    }

    public object Evaluate(Evaluator evaluator, Scope scope)
    {
        return new Reaction(Type, Description);
    }
}
