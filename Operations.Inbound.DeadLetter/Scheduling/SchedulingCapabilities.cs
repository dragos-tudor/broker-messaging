namespace Operations.Inbound.DeadLetter;

public sealed record SchedulingCapabilities<TKey, TPayload>(
  UpdateDeadLetterMessageAsync<TKey, TPayload, SchedulingUpdate>
    UpdateDeadLetterMessageAsync,
  GetDeadLetterRetryOptions GetDeadLetterRetryOptions,
  GetUtcDateTime GetUtcDateTime
);
