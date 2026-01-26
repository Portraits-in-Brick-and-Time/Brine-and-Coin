using System.IO;
using System.Linq;
using LibObjectFile.Elf;
using MessagePack;

namespace ObjectModel;

internal abstract class CustomSection(ElfFile file)
{
    protected ElfStreamSection Section;
    protected ElfFile File => file;

    protected CustomSections CustomSections;

    private readonly ElfFile file = file;

    public abstract string Name { get; }

    protected virtual ElfSymbolType SymbolType { get; } = ElfSymbolType.Object;

    /// <summary>
    /// Prepares a new instance of the <see cref="CustomSection"/> class for writing.
    /// </summary>
    public void PrepareForWriting(CustomSections customSections)
    {
        CustomSections = customSections;

        Section = new ElfStreamSection(ElfSectionSpecialType.Text, new MemoryStream())
        {
            Name = Name,
            Flags = ElfSectionFlags.Alloc | ElfSectionFlags.Compressed
        };

        File.Add(Section);
    }

    /// <summary>
    /// Prepares a new instance of the <see cref="CustomSection"/> class for reading
    /// </summary>
    public void PrepareForReading(CustomSections customSections)
    {
        Section = (ElfStreamSection)file.Sections.First(_ => _.Name.Value == Name);
        CustomSections = customSections;
    }

    public void Write(CustomSections customSections)
    {
        PrepareForWriting(customSections);
        var writer = new BinaryWriter(Section.Stream);
        Write(writer);
    }

    public void Read(CustomSections customSections)
    {
        PrepareForReading(customSections);
        var reader = new BinaryReader(Section.Stream);
        Read(reader);
    }

    protected abstract void Write(BinaryWriter writer);
    protected abstract void Read(BinaryReader reader);

    protected void AddSymbol(string name, ulong index, ulong size, ElfSymbolTable symbolTable)
    {
        foreach (var symbol in symbolTable.Entries)
        {
            if (symbol.Name == name)
            {
                return;
            }
        }

        symbolTable.Entries.Add(
            new ElfSymbol()
            {
                Bind = ElfSymbolBind.Global,
                Name = name,
                Value = index,
                Type = SymbolType,
                Size = size,
                SectionLink = Section
            });
    }
}