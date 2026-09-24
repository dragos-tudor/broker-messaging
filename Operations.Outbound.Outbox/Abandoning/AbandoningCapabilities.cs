namespace Operations.Outbound.Outbox;

sealed record AbandoningCapabilities<TKey, TPayload>(
  UpdateOutboxMessageAsync<TKey, TPayload, AbandoningUpdate>
    UpdateOutboxMessageAsync
);
