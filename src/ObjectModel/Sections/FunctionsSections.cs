using LibObjectFile.Elf;
using ObjectModel.Models.Code;

namespace ObjectModel.Sections;

internal class FunctionsSection : ModelSection<FuncDefModel>
{
    public override string Name => ".functions";

    public FunctionsSection(ElfFile file) : base(file)
    {

    }

    public override void PrepareForWriting(CustomSections customSections)
    {
        base.PrepareForWriting(customSections);

        Section.Flags |= ElfSectionFlags.Executable;
    }
}