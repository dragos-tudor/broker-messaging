namespace Operations.Inbound.Inbox;

public sealed record DeadLetteringCapabilities(
  UpdateInboxMessageAsync< DeadLetteringUpdate>
    UpdateInboxMessageAsync
);
