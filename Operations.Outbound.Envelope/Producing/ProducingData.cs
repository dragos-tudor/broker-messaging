namespace Operations.Outbound.Envelope;

readonly record struct ProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  IEnvelope<TKey, TValue, TMetadata, TConfirmation>? Envelope,
  IOutboxMessage<TKey, TPayload>? OutboxMessage
);
