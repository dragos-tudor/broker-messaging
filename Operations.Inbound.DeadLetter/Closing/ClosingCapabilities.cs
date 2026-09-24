namespace Operations.Inbound.DeadLetter;

sealed record ClosingCapabilities<TKey, TPayload>(
  UpdateDeadLetterMessageAsync<TKey, TPayload, ClosingUpdate>
    UpdateDeadLetterMessageAsync
);
