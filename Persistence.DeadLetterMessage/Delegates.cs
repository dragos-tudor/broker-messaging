namespace Persistence.DeadLetterMessage;

public delegate DeadLetterRetryOptions GetDeadLetterRetryOptions();

public delegate Task<bool> InsertDeadLetterMessageAsync(
  IDeadLetterMessage message,
  CancellationToken ct = default
);

public delegate Task UpdateDeadLetterMessageAsync<TParam>(
  IDeadLetterMessage message,
  TParam parameters,
  CancellationToken ct = default
)
where TParam : struct;
