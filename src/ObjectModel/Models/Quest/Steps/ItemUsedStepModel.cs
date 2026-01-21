using MessagePack;

namespace ObjectModel.Models.Quest.Steps;

[MessagePackObject(AllowPrivate = true)]
internal class ItemUsedStepModel : IQuestStepModel
{
    public ItemUsedStepModel(string itemName)
    {
        ItemName = itemName;
    }

    [Key(0)]
    public string ItemName { get; set; }
}
