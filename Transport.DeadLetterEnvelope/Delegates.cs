namespace Transport.DeadLetterEnvelope;

public delegate Task PublishDeadLetterEnvelopeAsync(
  IDeadLetterEnvelope envelope,
  CancellationToken ct = default
);

public delegate bool ProduceDeadLetterEnvelope(
  IDeadLetterEnvelope envelope,
  Action<bool, Exception?> dispatcher
);
