namespace Operations.Inbound.DeadLetter;

public sealed record InsertingCapabilities<TKey, TPayload>(
  InsertDeadLetterMessageAsync<TKey, TPayload> InsertDeadLetterMessageAsync
);
