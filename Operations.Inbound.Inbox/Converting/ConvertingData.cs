namespace Operations.Inbound.Inbox;

readonly record struct ConvertingData<TKey, TPayload>(
  IInboxMessage<TKey, TPayload>? InboxMessage,
  IDeadLetterMessage<TKey, TPayload>? DeadLetterMessage
);
