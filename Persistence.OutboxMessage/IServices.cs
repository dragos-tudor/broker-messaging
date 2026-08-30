
namespace Persistence.OutboxMessage;

public interface IOutboxMessageInsertSessionService<TKey, TPayload, TSession>
{
  Task<bool> InsertOutboxMessageAsync(
    TSession session,
    OutboxMessage<TKey, TPayload> message,
    CancellationToken ct = default);
}

public interface IOutboxMessagePayloadMapperService<TPayload, TValue> {
  TValue FromOutboxMessagePayload(TPayload payload);
}

public interface IOutboxMessageOptionsReaderService {
  OutboxMessageOptions GetOutboxMessageOptions();
}

public interface IOutboxMessageUpdateService<TKey, TPayload>
{
  Task UpdateOutboxMessageAsync<TMessage>(
    TMessage message,
    Func<TMessage, TMessage> update,
    CancellationToken ct = default) where TMessage : OutboxMessage<TKey, TPayload>;
}