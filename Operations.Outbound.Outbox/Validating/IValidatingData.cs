
namespace Operations.Outbound.Outbox;

public interface IValidatingData<TKey, TPayload>:
  IOutboxMessageProp<TKey, TPayload>;