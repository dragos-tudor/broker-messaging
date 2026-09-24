namespace Operations.Inbound.DeadLetterEnvelope;

readonly record struct ProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? DeadLetterEnvelope,
  IDeadLetterMessage<TKey, TPayload>? DeadLetterMessage
);
