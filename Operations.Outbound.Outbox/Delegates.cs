namespace Operations.Outbound.Outbox;

public delegate IEnvelope<TKey, TValue, TMetadata, TConfirmation>
  FromOutboxMessage<TKey, TValue, TMetadata, TConfirmation, TPayload>(
    IOutboxMessage<TKey, TPayload> message,
    DateTime currentDate
  );
