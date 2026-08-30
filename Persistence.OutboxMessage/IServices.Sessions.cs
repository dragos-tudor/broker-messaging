
namespace Persistence.OutboxMessage;

public interface IOutboxSessionServices<TSession>:
  IOutboxSessionModelPersistService<TSession>,
  IOutboxSessionReaderService<TSession>,
  IOutboxSessionTransactService<TSession>
  where TSession : IDisposable;

public interface IOutboxSessionModelPersistService<TSession> where TSession: IDisposable {
  Task PersistOutboxModelAsync<TModel>(TSession session, TModel model);
}

public interface IOutboxSessionReaderService<TSession> where TSession : IDisposable {
  TSession GetSession();
}

public interface IOutboxSessionTransactService<TSession> where TSession: IDisposable {
  Task TransactSessionAsync(
    TSession session,
    Func<TSession, Task> func1,
    Func<TSession, Task> func2,
    CancellationToken ct = default
  );
}