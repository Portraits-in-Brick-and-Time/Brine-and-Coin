using MessagePack;

namespace ObjectModel.Models.Quest.Steps;

[MessagePackObject(AllowPrivate = true)]
internal class RegionEnteredStepModel : IQuestStepModel
{
    public RegionEnteredStepModel(string regionName)
    {
        RegionName = regionName;
    }

    [Key(0)]
    public string RegionName { get; set; }
}
