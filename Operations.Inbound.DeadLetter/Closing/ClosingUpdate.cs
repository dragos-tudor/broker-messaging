
namespace Operations.Inbound.DeadLetter;

public readonly record struct ClosingUpdate(
  DeadLetterMessageStatus Status
);