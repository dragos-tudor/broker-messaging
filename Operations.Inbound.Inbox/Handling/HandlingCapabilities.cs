
namespace Operations.Inbound.Inbox;

public sealed record HandlingCapabilities (
  HandleInboxMessageAsync HandleInboxMessageAsync
);