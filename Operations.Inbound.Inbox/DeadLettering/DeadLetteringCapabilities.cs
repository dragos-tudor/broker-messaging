namespace Operations.Inbound.Inbox;

public sealed record DeadLetteringCapabilities<TKey, TPayload>(
  UpdateInboxMessageAsync<TKey, TPayload, DeadLetteringUpdate>
    UpdateInboxMessageAsync
);
