namespace Operations.Inbound.Envelope;

sealed record CapturingCapabilities<TKey, TValue, TMetadata, TConfirmation>(
  ReadEnvelope<TKey, TValue, TMetadata, TConfirmation> ReadEnvelope
);
