using MessagePack;

namespace ObjectModel.Models.Quest.Steps;

[MessagePackObject(AllowPrivate = true)]
internal class ItemRemovedStepModel : IQuestStepModel
{
    [Key(0)]
    public string ItemName { get; }
}
