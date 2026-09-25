namespace Operations.Inbound.DeadLetter;

public sealed record ClosingCapabilities<TKey, TPayload>(
  UpdateDeadLetterMessageAsync<TKey, TPayload, ClosingUpdate>
    UpdateDeadLetterMessageAsync
);
