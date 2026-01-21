using LibObjectFile.Elf;
using ObjectModel.Sections;

namespace ObjectModel;

internal class CustomSections
{
    public AttributesSection AttributesSection { get; }
    public ItemsSection ItemsSection { get; }

    public CharactersSection CharactersSection { get; }

    public RoomsSection RoomsSection { get; }

    public RegionsSection RegionsSection { get; }

    public MetaSection MetaSection { get; }

    public QuestsSection QuestsSection { get; }

    private CustomSection[] _allSections;

    public CustomSections(ElfFile file)
    {
        AttributesSection = new(file);
        ItemsSection = new(file);
        CharactersSection = new(file);
        RoomsSection = new(file);
        RegionsSection = new(file);
        QuestsSection = new(file);

        MetaSection = new(file);

        _allSections =
        [
            MetaSection,
            AttributesSection,
            ItemsSection,
            RoomsSection,
            CharactersSection,
            RegionsSection,
            QuestsSection
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