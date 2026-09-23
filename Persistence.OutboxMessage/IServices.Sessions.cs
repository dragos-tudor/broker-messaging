
namespace Persistence.OutboxMessage;

public interface IOutboxSessionServices<TSession>:
  IOutboxSessionReaderService<TSession>,
  IOutboxSessionModelPersistService<TSession>,
  IOutboxSessionTransactService<TSession>
  where TSession : IDisposable;

public interface IOutboxSessionModelPersistService<TSession> where TSession: IDisposable {
  Task PersistDomainModelAsync<TModel>(
    TSession session,
    TModel model,
    CancellationToken ct = default);
}

public interface IOutboxSessionReaderService<TSession> where TSession : IDisposable {
  TSession GetSession();
}

public interface IOutboxSessionTransactService<TSession> where TSession: IDisposable {
  Task TransactSessionAsync<TServices, TParams>(
    TServices services,
    TSession session,
    TParams parameters,
    Func<TServices, TSession, TParams, CancellationToken, Task> func1,
    Func<TServices, TSession, TParams, CancellationToken, Task> func2,
    CancellationToken ct = default
  )
  where TServices : IOutboxSessionServices<TSession>
  where TParams : struct;
}