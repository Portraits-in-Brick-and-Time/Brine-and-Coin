namespace BrineAndCoin.Questing;

public interface IQuestStep
{
    bool IsCompleted { get; }
    void OnEvent(NetAF.Logging.Events.BaseEvent gameEvent);
}
