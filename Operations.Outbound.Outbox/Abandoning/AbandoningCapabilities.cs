namespace Operations.Outbound.Outbox;

public sealed record AbandoningCapabilities(
  UpdateOutboxMessageAsync<AbandoningUpdate> UpdateOutboxMessageAsync
);
