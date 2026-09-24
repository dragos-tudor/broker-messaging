namespace Operations.Inbound.Envelope;

sealed record MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  FromEnvelopeToInboxMessage<TKey, TValue, TMetadata, TConfirmation, TPayload>
    FromEnvelope,
  GetUtcDateTime GetUtcDateTime
);
