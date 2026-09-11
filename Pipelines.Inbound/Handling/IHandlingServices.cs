using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public interface IHandlingServices<TKey, TPayload, TSession>:
  IHandlingServices<TKey, TPayload>,
  ITransactingServices<TKey, TPayload, TSession>,
  ISchedulingServices<TKey, TPayload>,
  IAbandoningServices<TKey, TPayload>
  where TSession: IDisposable;
