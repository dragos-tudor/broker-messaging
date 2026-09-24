namespace Operations.Outbound.Outbox;

readonly record struct TransactingData<TKey, TPayload>(
  IOutboxMessage<TKey, TPayload>? OutboxMessage,
  object? Model
);
