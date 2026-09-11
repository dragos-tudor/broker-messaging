
namespace Operations.Outbound.Outbox;

public readonly record struct ClosingUpdate(
  OutboxMessageStatus Status
);