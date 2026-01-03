using LibObjectFile.Elf;
using ObjectModel.Sections;

namespace ObjectModel;

public class CustomSections
{
    public AttributesSection AttributesSection { get; }
    public ItemsSection ItemsSection { get; }

    public CharactersSection CharactersSection { get; }

    public RoomsSection RoomsSection { get; }

    public RegionsSection RegionsSection { get; }

    private CustomSection[] _allSections;

    public CustomSections(ElfFile file)
    {
        AttributesSection = new(file);
        ItemsSection = new(file);
        CharactersSection = new(file);
        RoomsSection = new(file);
        RegionsSection = new(file);

        _allSections =
        [
            // No Dependencies To Other Sections
            AttributesSection,
            ItemsSection,

            // Depends On Other Sections
            RoomsSection,
            CharactersSection,

            // Dependend to RoomsSection
            RegionsSection
        ];
    }

    public void Write(ElfSymbolTable symbolTable)
    {
        foreach (var section in _allSections)
        {
            section.Write(symbolTable, this);
        }
    }
    
    public void Read()
    {
        foreach (var section in _allSections)
        {
            section.Read(this);
        }
    }
}