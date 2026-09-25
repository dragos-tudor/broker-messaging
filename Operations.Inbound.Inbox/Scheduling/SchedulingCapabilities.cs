namespace Operations.Inbound.Inbox;

public sealed record SchedulingCapabilities<TKey, TPayload>(
  UpdateInboxMessageAsync<TKey, TPayload, SchedulingUpdate>
    UpdateInboxMessageAsync,
  GetInboxRetryOptions GetInboxRetryOptions,
  GetUtcDateTime GetUtcDateTime
);
