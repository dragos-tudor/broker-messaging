namespace Operations.Inbound.Inbox;

readonly record struct AbandoningData<TKey, TPayload>(
  IInboxMessage<TKey, TPayload>? InboxMessage
);
