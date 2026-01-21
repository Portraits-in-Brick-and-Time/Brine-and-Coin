using MessagePack;

namespace ObjectModel.Models.Quest.Steps;

[MessagePackObject(AllowPrivate = true)]
internal class GoToRoomStepModel : IQuestStepModel
{
    public GoToRoomStepModel(string roomName)
    {
        RoomName = roomName;
    }

    [Key(0)]
    public string RoomName { get; set; }
}
