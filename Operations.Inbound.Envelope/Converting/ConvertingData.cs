namespace Operations.Inbound.Envelope;

readonly record struct ConvertingData<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  IEnvelope<TKey, TValue, TMetadata, TConfirmation>? Envelope,
  IInboxMessage<TKey, TPayload>? InboxMessage,
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? DeadLetterEnvelope
);
