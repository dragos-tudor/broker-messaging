namespace Operations.Outbound.Outbox;

readonly record struct ValidatingData<TKey, TPayload>(
  IOutboxMessage<TKey, TPayload>? OutboxMessage
);
