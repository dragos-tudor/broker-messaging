namespace Operations.Outbound.Outbox;

readonly record struct ClosingData<TKey, TPayload>(
  IOutboxMessage<TKey, TPayload>? OutboxMessage
);
