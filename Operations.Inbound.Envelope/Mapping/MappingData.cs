namespace Operations.Inbound.Envelope;

readonly record struct MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  IEnvelope<TKey, TValue, TMetadata, TConfirmation>? Envelope,
  IInboxMessage<TKey, TPayload>? InboxMessage
);
