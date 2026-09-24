namespace Operations.Inbound.DeadLetter;

sealed record AbandoningCapabilities<TKey, TPayload>(
  UpdateDeadLetterMessageAsync<TKey, TPayload, AbandoningUpdate>
    UpdateDeadLetterMessageAsync
);
