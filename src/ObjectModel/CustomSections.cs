using System.Linq;
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

    private readonly CustomSection[] _allSections;

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

    public void Write()
    {
        foreach (var section in _allSections)
        {
            section.Write(this);
        }
    }

    public void Read()
    {
        foreach (var section in _allSections)
        {
            section.Read(this);
        }
    }

    public void PopulateSymbolTable(ElfSymbolTable symbolTable)
    {
        foreach (var section in _allSections
            .Where(s => s is ISymbolTablePopulatable)
            .OfType<ISymbolTablePopulatable>())
        {
            section.PopulateSymbolTable(symbolTable);
        }
    }
}