namespace Operations.Outbound.Outbox;

readonly record struct SchedulingData<TKey, TPayload>(
  IOutboxMessage<TKey, TPayload>? OutboxMessage
);
