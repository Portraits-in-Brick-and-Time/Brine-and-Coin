using System.Collections.Generic;
using Hocon;
using MessagePack;
using ObjectModel.Evaluation;

namespace ObjectModel.Models.Code;

[MessagePackObject(AllowPrivate = true)]
internal class ValueModel(object value) : IEvaluable
{
    [Key(0)]
    public object Value { get; set; } = value;

    public ValueModel() : this(null)
    {
    }

    public object Evaluate(Evaluator evaluator, Scope scope)
    {
        return Value;
    }

    private static ValueModel FromObject(HoconValue value)
    {
        switch (value.Type)
        {
            case HoconType.String:
                return new ValueModel(value.GetString());
            case HoconType.Number:
                return new ValueModel(value.GetNumber());
            case HoconType.Boolean:
                return new ValueModel(value.GetBoolean());
            case HoconType.Empty:
                return new ValueModel(null);
            case HoconType.Object:
                {
                    var dict = new Dictionary<string, object>();
                    var hoconObject = value.GetObject();

                    foreach (var child in hoconObject)
                    {
                        var childValue = FromObject(child);
                        dict[child.Key] = childValue;
                    }

                    return new ValueModel(dict);
                }
            case HoconType.Array:
                {
                    var list = new List<object>();
                    var hoconArray = value.GetArray();

                    foreach (var item in hoconArray)
                    {
                        var itemValue = FromObject(item);
                        list.Add(itemValue);
                    }

                    return new ValueModel(list);
                }
        }

        return new ValueModel(null);
    }

    public static IEvaluable FromObject(KeyValuePair<string, HoconField> rootObj)
    {
       return FromObject(rootObj.Value.Value);
    }
}
