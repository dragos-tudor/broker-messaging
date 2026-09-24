namespace Operations.Outbound.Outbox;

readonly record struct AbandoningData<TKey, TPayload>(
  IOutboxMessage<TKey, TPayload>? OutboxMessage
);
