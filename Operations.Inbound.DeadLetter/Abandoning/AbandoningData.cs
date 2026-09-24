namespace Operations.Inbound.DeadLetter;

readonly record struct AbandoningData<TKey, TPayload>(
  IDeadLetterMessage<TKey, TPayload>? DeadLetterMessage
);
