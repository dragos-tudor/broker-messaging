namespace Operations.Inbound.DeadLetter;

readonly record struct ClosingData<TKey, TPayload>(
  IDeadLetterMessage<TKey, TPayload>? DeadLetterMessage
);
