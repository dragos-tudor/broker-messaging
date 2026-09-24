namespace Operations.Inbound.Envelope;

readonly record struct VerifyingData<TKey, TValue, TMetadata, TConfirmation>(
  IEnvelope<TKey, TValue, TMetadata, TConfirmation>? Envelope
);
