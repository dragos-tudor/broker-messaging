namespace Operations.Inbound.Envelope;

public sealed record ConvertingCapabilities(
  FromEnvelopeToDeadLetterEnvelope
    FromEnvelope,
  GetUtcDateTime GetUtcDateTime
);
