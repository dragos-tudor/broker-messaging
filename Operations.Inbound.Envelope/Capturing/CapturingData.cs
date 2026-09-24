namespace Operations.Inbound.Envelope;

readonly record struct CapturingData<TKey, TValue, TMetadata, TConfirmation>(
  IEnvelope<TKey, TValue, TMetadata, TConfirmation>? Envelope
);
