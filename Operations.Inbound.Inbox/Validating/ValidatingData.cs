namespace Operations.Inbound.Inbox;

readonly record struct ValidatingData<TKey, TPayload>(
  IInboxMessage<TKey, TPayload>? InboxMessage
);
