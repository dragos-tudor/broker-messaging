namespace Operations.Inbound.DeadLetterEnvelope;

readonly record struct RedirectingData<TKey, TValue, TMetadata, TConfirmation>(
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? DeadLetterEnvelope
);
