using MessagePack;

namespace ObjectModel.Models.Quest.Steps;

[MessagePackObject(AllowPrivate = true)]
internal class RoomExitedStepModel : IQuestStepModel
{
    [Key(0)]
    public string RoomName { get; }
}