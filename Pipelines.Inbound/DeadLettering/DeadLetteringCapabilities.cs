using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public sealed record DeadLetteringCapabilities<TKey, TPayload>
(
  ConvertingCapabilities<TKey, TPayload> Converting,
  Operations.Inbound.DeadLetter.InsertingCapabilities<TKey, TPayload> Inserting,
  AbandoningCapabilities<TKey, TPayload> Abandoning,
  ClosingCapabilities<TKey, TPayload> Closing
);
