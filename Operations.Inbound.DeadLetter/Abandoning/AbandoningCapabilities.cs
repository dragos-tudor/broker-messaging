namespace Operations.Inbound.DeadLetter;

public sealed record AbandoningCapabilities<TKey, TPayload>(
  UpdateDeadLetterMessageAsync<TKey, TPayload, AbandoningUpdate>
    UpdateDeadLetterMessageAsync
);
