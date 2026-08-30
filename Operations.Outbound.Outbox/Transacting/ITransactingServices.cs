
namespace Operations.Outbound.Outbox;

public interface ITransactingServices<TKey, TPayload, TSession> :
  IOutboxMessageInsertSessionService<TKey, TPayload, TSession>,
  IOutboxSessionServices<TSession>
  where TSession : IDisposable;

