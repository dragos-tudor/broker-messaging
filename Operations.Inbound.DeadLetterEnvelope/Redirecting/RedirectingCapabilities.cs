namespace Operations.Inbound.DeadLetterEnvelope;

sealed record RedirectingCapabilities<TKey, TValue, TMetadata, TConfirmation>(
  PublishDeadLetterEnvelopeAsync<TKey, TValue, TMetadata, TConfirmation>
    PublishDeadLetterEnvelopeAsync
);
