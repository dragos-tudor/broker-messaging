
namespace Operations.Inbound.Inbox;

public interface IConvertingData<TKey, TPayload>:
  IDeadLetterMessageProp<TKey, TPayload>,
  IInboxMessageProp<TKey, TPayload>;