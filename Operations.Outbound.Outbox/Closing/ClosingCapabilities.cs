namespace Operations.Outbound.Outbox;

sealed record ClosingCapabilities<TKey, TPayload>(
  UpdateOutboxMessageAsync<TKey, TPayload, ClosingUpdate>
    UpdateOutboxMessageAsync
);
