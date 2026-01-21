using MessagePack;

namespace ObjectModel.Models.Quest.Steps;

[MessagePackObject(AllowPrivate = true)]
internal class CharacterDiesStepModel : IQuestStepModel
{
    [Key(0)]
    public string CharacterName { get; }
}
