
namespace Operations.Inbound.Inbox;

readonly record struct TransactingData<TKey, TPayload>
(
  IInboxMessage<TKey, TPayload>? InboxMessage,
  object? Model
);
