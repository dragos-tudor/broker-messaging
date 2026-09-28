namespace Operations.Inbound.DeadLetter;

public sealed record ClosingCapabilities(
  UpdateDeadLetterMessageAsync<ClosingUpdate>
    UpdateDeadLetterMessageAsync
);
