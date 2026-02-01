using NetAF.Logging.Events;

namespace BrineAndCoin.Questing;

public record QuestCompletedEvent(string Name) : BaseEvent
{
}
