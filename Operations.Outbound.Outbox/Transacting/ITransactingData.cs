
namespace Operations.Outbound.Outbox;

public interface ITransactingData<TKey, TPayload>:
  IModelProp,
  IOutboxMessageProp<TKey, TPayload>;