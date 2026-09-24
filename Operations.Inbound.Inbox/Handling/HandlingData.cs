
namespace Operations.Inbound.Inbox;

readonly record struct HandlingData<TKey, TPayload> (
  IInboxMessage<TKey, TPayload>? InboxMessage,
  object? Model = default
);