namespace Operations.Inbound.Envelope;

public sealed record ConfirmingCapabilities<TKey, TValue, TMetadata, TConfirmation>(
  ConfirmEnvelope<TKey, TValue, TMetadata, TConfirmation> ConfirmEnvelope
);
