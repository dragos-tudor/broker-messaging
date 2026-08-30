
namespace Operations.Outbound.Outbox;

public interface IClosingData<TKey, TPayload>:
  IOutboxMessageProp<TKey, TPayload>;