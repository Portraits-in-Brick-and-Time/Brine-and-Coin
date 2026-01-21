using System.Collections.Generic;
using MessagePack;
using ObjectModel.Models;

namespace ObjectModel.Models.Quest;

[MessagePackObject(AllowPrivate = true)]
internal class QuestModel : GameObjectModel
{
   [Key(4)]
   public List<IQuestStepModel> Steps { get; } = [];
}
