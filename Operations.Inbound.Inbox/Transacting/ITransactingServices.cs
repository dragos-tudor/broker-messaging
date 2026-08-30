
namespace Operations.Inbound.Inbox;

public interface ITransactingServices<TKey, TPayload, TSession> :
  IInboxMessageUpdateService<TKey, TPayload>,
  IInboxMessageUpdateSessionService<TKey, TPayload, TSession>,
  IInboxSessionServices<TSession>
  where TSession : IDisposable;
