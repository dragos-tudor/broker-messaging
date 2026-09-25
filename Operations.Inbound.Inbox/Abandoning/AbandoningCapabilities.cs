namespace Operations.Inbound.Inbox;

public sealed record AbandoningCapabilities<TKey, TPayload>(
  UpdateInboxMessageAsync<TKey, TPayload, AbandoningUpdate>
    UpdateInboxMessageAsync
);
