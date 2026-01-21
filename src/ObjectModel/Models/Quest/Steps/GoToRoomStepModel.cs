using MessagePack;

namespace ObjectModel.Models.Quest.Steps;

[MessagePackObject(AllowPrivate = true)]
internal class GoToRoomStepModel : IQuestStepModel
{
    [Key(0)]
    public string RoomName { get; }
}
