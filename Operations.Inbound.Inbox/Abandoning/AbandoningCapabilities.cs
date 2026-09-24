namespace Operations.Inbound.Inbox;

sealed record AbandoningCapabilities<TKey, TPayload>(
  UpdateInboxMessageAsync<TKey, TPayload, AbandoningUpdate>
    UpdateInboxMessageAsync
);
