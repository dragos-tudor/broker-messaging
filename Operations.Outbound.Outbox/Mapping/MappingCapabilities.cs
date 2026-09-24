namespace Operations.Outbound.Outbox;

sealed record MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  FromOutboxMessage<TKey, TValue, TMetadata, TConfirmation, TPayload>
    FromOutboxMessage
);
