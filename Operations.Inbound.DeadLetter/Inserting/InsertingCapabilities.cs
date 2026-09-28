namespace Operations.Inbound.DeadLetter;

public sealed record InsertingCapabilities(
  InsertDeadLetterMessageAsync InsertDeadLetterMessageAsync
);
