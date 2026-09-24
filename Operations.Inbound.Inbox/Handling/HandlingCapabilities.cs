
namespace Operations.Inbound.Inbox;

sealed record HandlingCapabilities<TKey, TPayload>
(
  HandleInboxMessageAsync<TKey, TPayload> HandleInboxMessageAsync
);