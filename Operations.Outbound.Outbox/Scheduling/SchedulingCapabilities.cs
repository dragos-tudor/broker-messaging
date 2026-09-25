namespace Operations.Outbound.Outbox;

public sealed record SchedulingCapabilities<TKey, TPayload>(
  UpdateOutboxMessageAsync<TKey, TPayload, SchedulingUpdate>
    UpdateOutboxMessageAsync,
  GetOutboxRetryOptions GetOutboxRetryOptions,
  GetUtcDateTime GetUtcDateTime
);
