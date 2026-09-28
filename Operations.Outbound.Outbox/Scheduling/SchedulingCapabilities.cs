namespace Operations.Outbound.Outbox;

public sealed record SchedulingCapabilities(
  UpdateOutboxMessageAsync<SchedulingUpdate> UpdateOutboxMessageAsync,
  GetOutboxRetryOptions GetOutboxRetryOptions,
  GetUtcDateTime GetUtcDateTime
);
