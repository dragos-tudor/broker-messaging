
namespace Operations.Inbound.DeadLetter;

public readonly record struct SchedulingUpdate(
  int RetryCount,
  DateTimeOffset NextAttemptAt,
  DeadLetterMessageStatus Status,
  string? LastError
);