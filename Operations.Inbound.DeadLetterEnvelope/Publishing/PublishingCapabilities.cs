namespace Operations.Inbound.DeadLetterEnvelope;

sealed record PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation>(
  PublishDeadLetterEnvelopeAsync<TKey, TValue, TMetadata, TConfirmation>
    PublishDeadLetterEnvelopeAsync
);
