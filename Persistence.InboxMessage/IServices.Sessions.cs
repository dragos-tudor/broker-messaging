
namespace Persistence.InboxMessage;

public interface IInboxSessionServices<TSession>:
  IInboxSessionReaderService<TSession>,
  IInboxSessionModelStoreService<TSession>,
  IInboxSessionTransactService<TSession>
  where TSession : IDisposable;

public interface IInboxSessionModelStoreService<TSession> where TSession: IDisposable {
  Task StoreDomainModelAsync<TModel>(
    TSession session,
    TModel model,
    CancellationToken ct = default);
}

public interface IInboxSessionReaderService<TSession> where TSession : IDisposable {
  TSession GetSession();
}

public interface IInboxSessionTransactService<TSession> where TSession: IDisposable {
  Task TransactSessionAsync<TServices, TParams>(
    TServices services,
    TSession session,
    TParams parameters,
    Func<TServices, TSession, TParams, CancellationToken, Task> func1,
    Func<TServices, TSession, TParams, CancellationToken, Task> func2,
    CancellationToken ct = default
  ) where TServices : IInboxSessionServices<TSession>
    where TParams : struct;
}