using NetAF.Events;

namespace BrineAndCoin.Core.Questing;

public record QuestCompletedEvent(string Name) : BaseEvent
{
}
