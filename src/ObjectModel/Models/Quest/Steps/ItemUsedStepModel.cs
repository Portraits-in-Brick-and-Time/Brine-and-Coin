using MessagePack;

namespace ObjectModel.Models.Quest.Steps;

[MessagePackObject(AllowPrivate = true)]
internal class ItemUsedStepModel : IQuestStepModel
{
    [Key(0)]
    public string ItemName { get; }
}
