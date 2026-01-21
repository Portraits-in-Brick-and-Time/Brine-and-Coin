using MessagePack;

namespace ObjectModel.Models.Quest.Steps;

[MessagePackObject(AllowPrivate = true)]
internal class RegionExitedStepModel : IQuestStepModel
{
    [Key(0)]
    public string RegionName { get; }
}
