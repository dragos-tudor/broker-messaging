namespace Persistence.DeadLetterMessage;

public delegate DeadLetterRetryOptions GetDeadLetterRetryOptions();

public delegate Task<bool> InsertDeadLetterMessageAsync<TKey, TPayload>(
  IDeadLetterMessage<TKey, TPayload> message,
  CancellationToken ct = default
);

public delegate Task UpdateDeadLetterMessageAsync<TKey, TPayload, TParam>(
  IDeadLetterMessage<TKey, TPayload> message,
  TParam parameters,
  CancellationToken ct = default
)
where TParam : struct;
