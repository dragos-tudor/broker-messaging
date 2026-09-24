namespace Operations.Inbound.DeadLetter;

readonly record struct MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  IDeadLetterMessage<TKey, TPayload>? DeadLetterMessage,
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? DeadLetterEnvelope
);
