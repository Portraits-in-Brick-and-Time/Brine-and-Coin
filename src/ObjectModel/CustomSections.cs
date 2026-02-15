using System;
using System.Linq;
using LibObjectFile.Elf;
using ObjectModel.Evaluation;
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

    public FunctionsSection FunctionsSection { get; }

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
        FunctionsSection = new(file);

        _allSections =
        [
            MetaSection,
            FunctionsSection,
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
        foreach (var section in _allSections.OfType<ISymbolTablePopulatable>())
        {
            section.PopulateSymbolTable(symbolTable);
        }
    }
}