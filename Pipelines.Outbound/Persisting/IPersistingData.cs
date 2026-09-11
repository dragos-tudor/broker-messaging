using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

public interface IPersistingData<TKey, TPayload>:
  IValidatingData<TKey, TPayload>,
  ITransactingData<TKey, TPayload>;
