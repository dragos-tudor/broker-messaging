namespace Operations.Inbound.Inbox;

public sealed record SchedulingCapabilities(
  UpdateInboxMessageAsync< SchedulingUpdate>
    UpdateInboxMessageAsync,
  GetInboxRetryOptions GetInboxRetryOptions,
  GetUtcDateTime GetUtcDateTime
);
