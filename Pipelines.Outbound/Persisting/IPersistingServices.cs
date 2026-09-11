using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

public interface IPersistingServices<TKey, TPayload, TSession>:
  IValidatingServices<TKey, TPayload>,
  ITransactingServices<TKey, TPayload, TSession>
  where TSession: IDisposable;
