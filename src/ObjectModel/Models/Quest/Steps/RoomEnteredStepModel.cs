using MessagePack;

namespace ObjectModel.Models.Quest.Steps;

[MessagePackObject(AllowPrivate = true)]
internal class RoomEnteredStepModel : IQuestStepModel
{
    public RoomEnteredStepModel(string roomName)
    {
        RoomName = roomName;
    }

    [Key(0)]
    public string RoomName { get; set; }
}
