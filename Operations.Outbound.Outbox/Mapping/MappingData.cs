namespace Operations.Outbound.Outbox;

readonly record struct MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  IOutboxMessage<TKey, TPayload>? OutboxMessage,
  IEnvelope<TKey, TValue, TMetadata, TConfirmation>? Envelope
);
