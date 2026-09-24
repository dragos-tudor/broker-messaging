namespace Transport.DeadLetterEnvelope;

public delegate Task PublishDeadLetterEnvelopeAsync<TKey, TValue, TMetadata, TConfirmation>(
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
  CancellationToken ct = default
);

public delegate bool ProduceDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>(
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
  Action<bool, Exception?> dispatcher
);
