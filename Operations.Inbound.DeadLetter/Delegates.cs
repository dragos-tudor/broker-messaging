namespace Operations.Inbound.DeadLetter;

public delegate IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>
  FromDeadLetterMessage<TKey, TValue, TMetadata, TConfirmation, TPayload>(
    IDeadLetterMessage<TKey, TPayload> message,
    DateTime currentDate
  );
