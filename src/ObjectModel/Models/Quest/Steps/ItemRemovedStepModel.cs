using MessagePack;

namespace ObjectModel.Models.Quest.Steps;

[MessagePackObject(AllowPrivate = true)]
internal class ItemRemovedStepModel : IQuestStepModel
{
    public ItemRemovedStepModel(string itemName)
    {
        ItemName = itemName;
    }

    [Key(0)]
    public string ItemName { get; set; }
}
