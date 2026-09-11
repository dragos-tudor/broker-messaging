
namespace Operations.Inbound.Inbox;

public readonly record struct ClosingUpdate(
  InboxMessageStatus Status
);