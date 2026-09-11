
namespace Operations.Outbound.Outbox;

public interface ITransactingData<TKey, TPayload>:
  IDomainModelProp,
  IOutboxMessageProp<TKey, TPayload>;