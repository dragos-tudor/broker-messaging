namespace Operations.Inbound.DeadLetter;

public sealed record AbandoningCapabilities(
  UpdateDeadLetterMessageAsync<AbandoningUpdate>
    UpdateDeadLetterMessageAsync
);
