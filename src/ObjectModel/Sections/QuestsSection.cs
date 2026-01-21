using LibObjectFile.Elf;

namespace ObjectModel.Sections;

internal class QuestsSection : ModelSection<Models.Quest.QuestModel>
{
    public QuestsSection(ElfFile file) : base(file)
    {
    }

    public override string Name => "quests";
}