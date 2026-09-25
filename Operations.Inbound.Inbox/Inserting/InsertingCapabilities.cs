namespace Operations.Inbound.Inbox;

public sealed record InsertingCapabilities<TKey, TPayload>(
  InsertInboxMessageAsync<TKey, TPayload> InsertInboxMessageAsync
);
