namespace Transport.Envelope;

public delegate Task ConfirmEnvelope<TKey, TValue, TMetadata, TConfirmation>(
  IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
  CancellationToken ct = default
);

public delegate Task PublishEnvelopeAsync<TKey, TValue, TMetadata, TConfirmation>(
  IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
  CancellationToken ct = default
);

public delegate bool ProduceEnvelope<TKey, TValue, TMetadata, TConfirmation>(
  IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
  Action<bool, Exception?> dispatcher
);

public delegate Task<IEnvelope<TKey, TValue, TMetadata, TConfirmation>> ReadEnvelope<TKey, TValue, TMetadata, TConfirmation>(
  CancellationToken ct = default
);

