
namespace Operations.Inbound.Inbox;

public interface ISchedulingData<TKey, TPayload>:
  IInboxMessageProp<TKey, TPayload>;