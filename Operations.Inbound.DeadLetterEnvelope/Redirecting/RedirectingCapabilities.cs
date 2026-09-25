namespace Operations.Inbound.DeadLetterEnvelope;

public sealed record RedirectingCapabilities<TKey, TValue, TMetadata, TConfirmation>(
  PublishDeadLetterEnvelopeAsync<TKey, TValue, TMetadata, TConfirmation>
    PublishDeadLetterEnvelopeAsync
);
