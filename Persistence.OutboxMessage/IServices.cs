
namespace Persistence.OutboxMessage;

public interface IOutboxMessageInsertSessionService<TKey, TPayload, TSession>
{
  Task<bool> InsertOutboxMessageAsync(
    TSession session,
    IOutboxMessage<TKey, TPayload> message,
    CancellationToken ct = default);
}

public interface IOutboxRetryOptionsService {
  OutboxRetryOptions GetOutboxRetryOptions();
}

public interface IOutboxMessageUpdateService<TKey, TPayload>
{
  Task UpdateOutboxMessageAsync<TParams>(
    IOutboxMessage<TKey, TPayload> message,
    TParams parameters,
    CancellationToken ct = default)
  where TParams : struct;
}