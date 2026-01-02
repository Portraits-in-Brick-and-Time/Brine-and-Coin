using System.IO;
using System.Linq;
using LibObjectFile.Elf;

namespace ObjectModel;

public abstract class CustomSection(ElfFile file)
{
    protected ElfStreamSection Section;
    protected ElfFile File = file;
    private ElfSymbolTable _symbolTable;

    protected CustomSections CustomSections;

    public abstract string Name { get; }

    /// <summary>
    /// Prepares a new instance of the <see cref="CustomSection"/> class for writing.
    /// </summary>
    public void PrepareForWriting(ElfSymbolTable symbolTable, CustomSections customSections)
    {
        _symbolTable = symbolTable;
        CustomSections = customSections;

        Section = new ElfStreamSection(ElfSectionSpecialType.Text, new MemoryStream())
        {
            Name = Name,
            Flags = ElfSectionFlags.Alloc | ElfSectionFlags.Group | ElfSectionFlags.Compressed
        };
        
        File.Add(Section);
    }
    
    /// <summary>
    /// Prepares a new instance of the <see cref="CustomSection"/> class for reading
    /// </summary>
    public void PrepareForReading(CustomSections customSections)
    {
        Section = (ElfStreamSection)file.Sections.First(_ => _.Name.Value == Name);
        _symbolTable = (ElfSymbolTable)file.Sections.First(_ => _ is ElfSymbolTable);
        CustomSections = customSections;
    }

    public void Write(ElfSymbolTable symbolTable, CustomSections customSections)
    {
        PrepareForWriting(symbolTable, customSections);
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

    protected void AddSymbol(string name, ulong index, ulong size)
    {
        foreach (var symbol in _symbolTable.Entries)
        {
            if (symbol.Name == name)
            {
                return;
            }
        }

        _symbolTable.Entries.Add(
            new ElfSymbol()
            {
                Bind = ElfSymbolBind.Global,
                Name = name,
                Value = index,
                Type = ElfSymbolType.Object,
                Size = size,
                SectionLink = Section
            });
    }
}