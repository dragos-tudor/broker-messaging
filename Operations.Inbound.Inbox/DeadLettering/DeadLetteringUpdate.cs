
namespace Operations.Inbound.Inbox;

public readonly record struct DeadLetteringUpdate(
  InboxMessageStatus Status,
  string? LastError
);