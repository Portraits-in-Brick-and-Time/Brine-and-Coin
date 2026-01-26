namespace ObjectModel;

interface ISymbolTablePopulatable
{
    void PopulateSymbolTable(LibObjectFile.Elf.ElfSymbolTable symbolTable);
}