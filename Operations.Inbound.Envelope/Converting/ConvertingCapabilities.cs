namespace Operations.Inbound.Envelope;

sealed record ConvertingCapabilities<TKey, TValue, TMetadata, TConfirmation>(
  FromEnvelopeToDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>
    FromEnvelope,
  GetUtcDateTime GetUtcDateTime
);
