using System.IO;
using LibObjectFile.Elf;

namespace ObjectModel.Sections;

internal class CodeSection(ElfFile file) : CustomSection(file)
{
    public override string Name => ".code";

    public string Code { get; set; }

    public override void PrepareForWriting(CustomSections customSections)
    {
        base.PrepareForWriting(customSections);

        Section.Flags |= ElfSectionFlags.Executable;
    }

    protected override void Write(BinaryWriter writer)
    {
        writer.Write(Code);
    }

    protected override void Read(BinaryReader reader)
    {
        Code = reader.ReadString();
    }
}