
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

public interface IInboxRetryOptionsReaderService {
  InboxRetryOptions GetInboxRetryOptions();
}

public interface IInboxMessageUpdateService<TKey, TPayload>
{
  Task UpdateInboxMessageAsync<TMessage, TParam>(
    TMessage message,
    TParam parameters,
    CancellationToken ct = default)
  where TMessage : IInboxMessage<TKey, TPayload>
  where TParam : struct;
}

public interface IInboxMessageUpdateSessionService<TKey, TPayload, TSession>
  where TSession: IDisposable
{
  Task UpdateInboxMessageAsync<TMessage, TParam>(
    TSession session,
    TMessage message,
    TParam parameters,
    CancellationToken ct = default)
    where TMessage : IInboxMessage<TKey, TPayload>
    where TParam : struct;
}
