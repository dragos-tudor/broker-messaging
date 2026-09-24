namespace Operations.Inbound.DeadLetter;

sealed record SchedulingCapabilities<TKey, TPayload>(
  UpdateDeadLetterMessageAsync<TKey, TPayload, SchedulingUpdate>
    UpdateDeadLetterMessageAsync,
  GetDeadLetterRetryOptions GetDeadLetterRetryOptions,
  GetUtcDateTime GetUtcDateTime
);
