namespace Operations.Outbound.Outbox;

public sealed record ClosingCapabilities<TKey, TPayload>(
  UpdateOutboxMessageAsync<TKey, TPayload, ClosingUpdate>
    UpdateOutboxMessageAsync
);
