
namespace Persistence.InboxMessage;

public interface IInboxMessageHandlerService<TKey, TPayload>
{
  Task<(object?, string?)> HandleInboxMessageAsync(
    InboxMessage<TKey, TPayload> message,
    CancellationToken ct = default
  );
}

public interface IInboxMessageOptionsReaderService {
  InboxMessageOptions GetInboxMessageOptions();
}

public interface IInboxMessageUpdateService<TKey, TPayload>
{
  Task UpdateInboxMessageAsync<TMessage>(
    TMessage message,
    Func<TMessage, TMessage> update,
    CancellationToken ct = default) where TMessage : InboxMessage<TKey, TPayload>;
}

public interface IInboxMessageUpdateSessionService<TKey, TPayload, TSession>
  where TSession: IDisposable
{
  Task UpdateInboxMessageAsync<TMessage>(
    TSession session,
    TMessage message,
    Func<TMessage, TMessage> update,
    CancellationToken ct = default) where TMessage : InboxMessage<TKey, TPayload>;
}
