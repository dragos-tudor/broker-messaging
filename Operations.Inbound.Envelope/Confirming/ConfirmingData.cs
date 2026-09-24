namespace Operations.Inbound.Envelope;

readonly record struct ConfirmingData<TKey, TValue, TMetadata, TConfirmation>(
  IEnvelope<TKey, TValue, TMetadata, TConfirmation>? Envelope
);
