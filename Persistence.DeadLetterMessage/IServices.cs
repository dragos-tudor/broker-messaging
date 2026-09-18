
namespace Persistence.DeadLetterMessage;

public interface IDeadLetterMessageInsertService<TKey, TPayload>
{
  Task<bool> InsertDeadLetterMessageAsync(
    IDeadLetterMessage<TKey, TPayload> message,
    CancellationToken ct = default);
}

public interface IDeadLetterRetryOptionsService {
  DeadLetterRetryOptions GetDeadLetterRetryOptions();
}

public interface IDeadLetterMessageUpdateService<TKey, TPayload>
{
  Task UpdateDeadLetterMessageAsync<TMessage, TParams>(
    TMessage message,
    TParams parameters,
    CancellationToken ct = default)
  where TMessage : IDeadLetterMessage<TKey, TPayload>
  where TParams : struct;
}
