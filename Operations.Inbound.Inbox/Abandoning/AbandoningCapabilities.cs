namespace Operations.Inbound.Inbox;

public sealed record AbandoningCapabilities(
  UpdateInboxMessageAsync<AbandoningUpdate>
    UpdateInboxMessageAsync
);
