namespace Operations.Inbound.DeadLetter;

readonly record struct InsertingData<TKey, TPayload>(
  IDeadLetterMessage<TKey, TPayload>? DeadLetterMessage
);
