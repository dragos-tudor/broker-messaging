namespace Operations.Inbound.DeadLetter;

readonly record struct SchedulingData<TKey, TPayload>(
  IDeadLetterMessage<TKey, TPayload>? Message
);
