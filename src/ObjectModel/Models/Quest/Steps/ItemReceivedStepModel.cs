using MessagePack;

namespace ObjectModel.Models.Quest.Steps;

[MessagePackObject(AllowPrivate = true)]
internal class ItemReceivedStepModel : IQuestStepModel
{
    public ItemReceivedStepModel(string itemName)
    {
        ItemName = itemName;
    }

    [Key(0)]
    public string ItemName { get; set; }
}
