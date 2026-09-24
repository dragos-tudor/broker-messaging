namespace Operations.Inbound.Inbox;

sealed record ClosingCapabilities<TKey, TPayload>(
  UpdateInboxMessageAsync<TKey, TPayload, ClosingUpdate>
    UpdateInboxMessageAsync
);
