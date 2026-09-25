namespace Operations.Inbound.Envelope;

public sealed record CapturingCapabilities<TKey, TValue, TMetadata, TConfirmation>(
  ReadEnvelope<TKey, TValue, TMetadata, TConfirmation> ReadEnvelope
);
