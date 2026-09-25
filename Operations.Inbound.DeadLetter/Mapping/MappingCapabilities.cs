namespace Operations.Inbound.DeadLetter;

public sealed record MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  FromDeadLetterMessage<TKey, TValue, TMetadata, TConfirmation, TPayload>
    FromDeadLetterMessage
);
