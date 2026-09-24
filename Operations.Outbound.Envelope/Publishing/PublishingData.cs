namespace Operations.Outbound.Envelope;

readonly record struct PublishingData<TKey, TValue, TMetadata, TConfirmation>(
  IEnvelope<TKey, TValue, TMetadata, TConfirmation>? Envelope
);
