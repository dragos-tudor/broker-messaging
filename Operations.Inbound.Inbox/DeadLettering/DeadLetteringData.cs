namespace Operations.Inbound.Inbox;

readonly record struct DeadLetteringData<TKey, TPayload>(
  IInboxMessage<TKey, TPayload>? InboxMessage
);
