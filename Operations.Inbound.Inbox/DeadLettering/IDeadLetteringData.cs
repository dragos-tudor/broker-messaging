
namespace Operations.Inbound.Inbox;

public interface IDeadLetteringData<TKey, TPayload>:
  IInboxMessageProp<TKey, TPayload>;