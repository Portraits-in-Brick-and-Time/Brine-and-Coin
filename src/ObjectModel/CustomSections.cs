using LibObjectFile.Elf;
using ObjectModel.Sections;

namespace ObjectModel;

public class CustomSections
{
    public AttributesSection AttributesSection { get; }
    public ItemsSection ItemsSection { get; }

    public CharactersSection CharactersSection { get; }

    private CustomSection[] _allSections;

    public CustomSections(ElfFile file)
    {
        AttributesSection = new(file);
        ItemsSection = new(file);
        CharactersSection = new(file);

        _allSections =
        [
            AttributesSection,
            ItemsSection,
            CharactersSection
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