namespace Operations.Inbound.Envelope;

public sealed record MappingCapabilities(
  FromEnvelopeToInboxMessage
    FromEnvelope,
  GetUtcDateTime GetUtcDateTime
);
