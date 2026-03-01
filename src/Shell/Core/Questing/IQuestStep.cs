namespace BrineAndCoin.Core.Questing;

public interface IQuestStep
{
    bool IsCompleted { get; }
    void OnEvent(NetAF.Events.BaseEvent gameEvent);
}
