namespace Operations.Outbound.Outbox;

public sealed record ClosingCapabilities(
  UpdateOutboxMessageAsync<ClosingUpdate> UpdateOutboxMessageAsync
);
