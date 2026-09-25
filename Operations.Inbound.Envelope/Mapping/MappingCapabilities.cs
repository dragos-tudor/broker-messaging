namespace Operations.Inbound.Envelope;

public sealed record MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  FromEnvelopeToInboxMessage<TKey, TValue, TMetadata, TConfirmation, TPayload>
    FromEnvelope,
  GetUtcDateTime GetUtcDateTime
);
