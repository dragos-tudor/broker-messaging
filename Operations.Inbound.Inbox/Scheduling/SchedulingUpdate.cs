
namespace Operations.Inbound.Inbox;

public readonly record struct SchedulingUpdate(
  int RetryCount,
  DateTimeOffset NextAttemptAt,
  InboxMessageStatus Status,
  string? LastError,
  string? FailureReason
);