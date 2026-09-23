
namespace Persistence.InboxMessage;

public interface IInboxMessageHandlerService<TKey, TPayload>
{
  Task<(object?, string?)> HandleInboxMessageAsync(
    IInboxMessage<TKey, TPayload> message,
    CancellationToken ct = default
  );
}

public interface IInboxMessageInsertService<TKey, TPayload>
{
  Task<bool> InsertInboxMessageAsync(
    IInboxMessage<TKey, TPayload> message,
    CancellationToken ct = default
  );
}

public interface IInboxRetryOptionsService {
  InboxRetryOptions GetInboxRetryOptions();
}

public interface IInboxMessageUpdateService<TKey, TPayload>
{
  Task UpdateInboxMessageAsync<TParam>(
    IInboxMessage<TKey, TPayload> message,
    TParam parameters,
    CancellationToken ct = default)
  where TParam : struct;
}

public interface IInboxMessageUpdateSessionService<TKey, TPayload, TSession>
  where TSession: IDisposable
{
  Task UpdateInboxMessageAsync<TParam>(
    TSession session,
    IInboxMessage<TKey, TPayload> message,
    TParam parameters,
    CancellationToken ct = default)
    where TParam : struct;
}
