namespace Operations.Inbound.Inbox;

readonly record struct InsertingData<TKey, TPayload>(
  IInboxMessage<TKey, TPayload>? InboxMessage
);
