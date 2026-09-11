
namespace Operations.Outbound.Outbox;

public readonly record struct AbandoningUpdate(
  OutboxMessageStatus Status,
  string? LastError,
  string? FailureReason
);