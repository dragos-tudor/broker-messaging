
namespace Operations.Inbound.Inbox;

public sealed record HandlingCapabilities<TKey, TPayload>
(
  HandleInboxMessageAsync<TKey, TPayload> HandleInboxMessageAsync
);