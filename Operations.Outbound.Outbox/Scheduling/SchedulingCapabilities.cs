namespace Operations.Outbound.Outbox;

sealed record SchedulingCapabilities<TKey, TPayload>(
  UpdateOutboxMessageAsync<TKey, TPayload, SchedulingUpdate>
    UpdateOutboxMessageAsync,
  GetOutboxRetryOptions GetOutboxRetryOptions,
  GetUtcDateTime GetUtcDateTime
);
