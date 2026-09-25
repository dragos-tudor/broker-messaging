namespace Operations.Outbound.Outbox;

public sealed record MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  FromOutboxMessage<TKey, TValue, TMetadata, TConfirmation, TPayload>
    FromOutboxMessage
);
