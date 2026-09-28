namespace Transport.Envelope;

public delegate Task ConfirmEnvelope(
  IEnvelope envelope,
  CancellationToken ct = default
);

public delegate Task PublishEnvelopeAsync(
  IEnvelope envelope,
  CancellationToken ct = default
);

public delegate bool ProduceEnvelope(
  IEnvelope envelope,
  Action<bool, Exception?> dispatcher
);

public delegate Task<IEnvelope> ReadEnvelope(
  CancellationToken ct = default
);

public delegate string? ValidateEnvelope(IEnvelope envelope);

