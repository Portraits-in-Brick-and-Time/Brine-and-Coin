using MessagePack;

namespace ObjectModel.Models.Quest.Steps;

[MessagePackObject(AllowPrivate = true)]
internal class RegionEnteredStepModel : IQuestStepModel
{
    [Key(0)]
    public string RegionName { get; }
}
