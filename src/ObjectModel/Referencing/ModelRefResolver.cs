using LibObjectFile.Elf;
using MessagePack;
using MessagePack.Formatters;

namespace ObjectModel.Referencing;

public partial class ModelRefResolver : IFormatterResolver
{
    private readonly ModelRefFormatter _formatter = ModelRefFormatter.Instance;

    public IMessagePackFormatter<T> GetFormatter<T>()
    {
        if (typeof(T) == typeof(ModelRef))
            return (IMessagePackFormatter<T>)_formatter;

        return null;
    }
}