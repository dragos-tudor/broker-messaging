namespace Operations.Outbound.Envelope;

sealed record PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation>(
  PublishEnvelopeAsync<TKey, TValue, TMetadata, TConfirmation>
    PublishEnvelopeAsync
);
