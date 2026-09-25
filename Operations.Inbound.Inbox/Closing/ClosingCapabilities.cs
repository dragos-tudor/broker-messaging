namespace Operations.Inbound.Inbox;

public sealed record ClosingCapabilities<TKey, TPayload>(
  UpdateInboxMessageAsync<TKey, TPayload, ClosingUpdate>
    UpdateInboxMessageAsync
);
