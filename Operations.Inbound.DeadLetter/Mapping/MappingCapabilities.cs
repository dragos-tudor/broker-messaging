namespace Operations.Inbound.DeadLetter;

sealed record MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  FromDeadLetterMessage<TKey, TValue, TMetadata, TConfirmation, TPayload>
    FromDeadLetterMessage
);
