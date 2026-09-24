namespace Operations.Inbound.DeadLetterEnvelope;

readonly record struct PublishingData<TKey, TValue, TMetadata, TConfirmation>(
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? DeadLetterEnvelope
);
