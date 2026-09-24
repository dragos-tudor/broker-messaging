namespace Operations.Inbound.Inbox;

sealed record InsertingCapabilities<TKey, TPayload>(
  InsertInboxMessageAsync<TKey, TPayload> InsertInboxMessageAsync
);
