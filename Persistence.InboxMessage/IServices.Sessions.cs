
namespace Persistence.InboxMessage;

public interface IInboxSessionServices<TSession>:
  IInboxSessionReaderService<TSession>,
  IInboxSessionModelPersistService<TSession>,
  IInboxSessionTransactService<TSession>
  where TSession : IDisposable;

public interface IInboxSessionModelPersistService<TSession> where TSession: IDisposable {
  Task PersistInboxModelAsync<TModel>(TSession session, TModel model);
}

public interface IInboxSessionTransactService<TSession> where TSession: IDisposable {
  Task TransactSessionAsync(
    TSession session,
    Func<TSession, Task> func1,
    Func<TSession, Task> func2,
    CancellationToken ct = default
  );
}

public interface IInboxSessionReaderService<TSession> where TSession : IDisposable {
  TSession GetSession();
}