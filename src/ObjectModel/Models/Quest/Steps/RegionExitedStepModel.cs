using MessagePack;

namespace ObjectModel.Models.Quest.Steps;

[MessagePackObject(AllowPrivate = true)]
internal class RegionExitedStepModel : IQuestStepModel
{
    public RegionExitedStepModel(string regionName)
    {
        RegionName = regionName;
    }

    [Key(0)]
    public string RegionName { get; set; }
}
