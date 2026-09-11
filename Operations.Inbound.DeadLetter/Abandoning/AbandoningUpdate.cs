
namespace Operations.Inbound.DeadLetter;

public readonly record struct AbandoningUpdate(
  DeadLetterMessageStatus Status,
  string? LastError,
  string? FailureReason
);