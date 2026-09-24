namespace Operations.Inbound.Inbox;

sealed record DeadLetteringCapabilities<TKey, TPayload>(
  UpdateInboxMessageAsync<TKey, TPayload, DeadLetteringUpdate>
    UpdateInboxMessageAsync
);
