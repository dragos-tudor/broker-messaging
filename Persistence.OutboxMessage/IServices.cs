
namespace Persistence.OutboxMessage;

public interface IOutboxMessageInsertSessionService<TKey, TPayload, TSession>
{
  Task<bool> InsertOutboxMessageAsync(
    TSession session,
    IOutboxMessage<TKey, TPayload> message,
    CancellationToken ct = default);
}

public interface IOutboxMessageOptionsReaderService {
  OutboxMessageOptions GetOutboxMessageOptions();
}

public interface IOutboxMessageUpdateService<TKey, TPayload>
{
  Task UpdateOutboxMessageAsync<TMessage, TParams>(
    TMessage message,
    TParams parameters,
    CancellationToken ct = default)
  where TMessage : IOutboxMessage<TKey, TPayload>
  where TParams : struct;
}