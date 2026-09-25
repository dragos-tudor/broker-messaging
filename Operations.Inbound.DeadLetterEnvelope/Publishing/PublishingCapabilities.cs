namespace Operations.Inbound.DeadLetterEnvelope;

public sealed record PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation>(
  PublishDeadLetterEnvelopeAsync<TKey, TValue, TMetadata, TConfirmation>
    PublishDeadLetterEnvelopeAsync
);
