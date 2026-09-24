namespace Operations.Inbound.Inbox;

readonly record struct ClosingData<TKey, TPayload>(
  IInboxMessage<TKey, TPayload>? InboxMessage
);
