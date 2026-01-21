using MessagePack;
using ObjectModel.Models.Quest.Steps;

namespace ObjectModel.Models.Quest;

[Union(0, typeof(CharacterDiesStepModel))]
[Union(1, typeof(GoToRoomStepModel))]
[Union(2, typeof(ItemReceivedStepModel))]
[Union(3, typeof(ItemRemovedStepModel))]
[Union(4, typeof(ItemUsedStepModel))]
[Union(5, typeof(RegionEnteredStepModel))]
[Union(6, typeof(RegionExitedStepModel))]
[Union(7, typeof(RoomEnteredStepModel))]
[Union(8, typeof(RoomExitedStepModel))]
internal interface IQuestStepModel
{
}
