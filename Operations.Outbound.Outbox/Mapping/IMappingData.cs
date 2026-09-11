
namespace Operations.Outbound.Outbox;

public interface IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>,
  IOutboxMessageProp<TKey, TPayload>;