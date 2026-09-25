namespace Operations.Outbound.Envelope;

public sealed record PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation>(
  PublishEnvelopeAsync<TKey, TValue, TMetadata, TConfirmation>
    PublishEnvelopeAsync
);
