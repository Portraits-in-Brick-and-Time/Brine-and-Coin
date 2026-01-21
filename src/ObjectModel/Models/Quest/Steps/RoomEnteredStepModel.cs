using MessagePack;

namespace ObjectModel.Models.Quest.Steps;

[MessagePackObject(AllowPrivate = true)]
internal class RoomEnteredStepModel : IQuestStepModel
{
    [Key(0)]
    public string RoomName { get; }
}
