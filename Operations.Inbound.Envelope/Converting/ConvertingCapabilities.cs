namespace Operations.Inbound.Envelope;

public sealed record ConvertingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  FromEnvelopeToDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>
    FromEnvelope,
  GetUtcDateTime GetUtcDateTime
);
