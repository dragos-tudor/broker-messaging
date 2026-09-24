namespace Operations.Inbound.DeadLetter;

sealed record InsertingCapabilities<TKey, TPayload>(
  InsertDeadLetterMessageAsync<TKey, TPayload> InsertDeadLetterMessageAsync
);
