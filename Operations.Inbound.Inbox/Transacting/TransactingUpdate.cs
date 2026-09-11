
namespace Operations.Inbound.Inbox;

public readonly record struct TransactingUpdate(
  InboxMessageStatus Status
);