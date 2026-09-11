
namespace Operations.Inbound.Inbox;

public readonly record struct AbandoningUpdate(
  InboxMessageStatus Status,
  string? LastError,
  string? FailureReason
);