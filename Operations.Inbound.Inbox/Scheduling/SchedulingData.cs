namespace Operations.Inbound.Inbox;

readonly record struct SchedulingData<TKey, TPayload>(
  IInboxMessage<TKey, TPayload>? InboxMessage
);
