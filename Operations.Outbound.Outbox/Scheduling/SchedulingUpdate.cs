
namespace Operations.Outbound.Outbox;

public readonly record struct SchedulingUpdate(
  int RetryCount,
  DateTimeOffset NextAttemptAt,
  OutboxMessageStatus Status,
  string? LastError
);