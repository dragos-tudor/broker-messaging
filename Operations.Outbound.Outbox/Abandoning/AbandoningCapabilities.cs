namespace Operations.Outbound.Outbox;

public sealed record AbandoningCapabilities<TKey, TPayload>(
  UpdateOutboxMessageAsync<TKey, TPayload, AbandoningUpdate>
    UpdateOutboxMessageAsync
);
