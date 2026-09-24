namespace Operations.Inbound.Envelope;

sealed record ConfirmingCapabilities<TKey, TValue, TMetadata, TConfirmation>(
  ConfirmEnvelope<TKey, TValue, TMetadata, TConfirmation> ConfirmEnvelope
);
