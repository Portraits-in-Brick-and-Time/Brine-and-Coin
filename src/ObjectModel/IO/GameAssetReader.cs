namespace ObjectModel.IO;

using System.IO;
using System.Linq;
using LibObjectFile.Elf;
using MessagePack;
using MessagePack.Resolvers;
using ObjectModel.Referencing;

internal class GameAssetReader
{
    private ElfFile _file;

    public readonly ElfSymbolTable SymbolTable;
    public readonly CustomSections CustomSections;

    public ElfFile File { get => _file; }

    public GameAssetReader(Stream strm)
    {
        _file = ElfFile.Read(strm);

        SymbolTable = (ElfSymbolTable)File.Sections.First(_ => _ is ElfSymbolTable);

        ModelRefFormatter.Instance.SymbolTable = SymbolTable;
        var resolver = CompositeResolver.Create(
                    new ModelRefResolver(),
                    StandardResolver.Instance
                );

        MessagePackSerializer.DefaultOptions = MessagePackSerializer.DefaultOptions
            .WithResolver(resolver)
            .WithCompression(MessagePackCompression.Lz4Block);

        CustomSections = new(File);
        CustomSections.Read();
    }
}
