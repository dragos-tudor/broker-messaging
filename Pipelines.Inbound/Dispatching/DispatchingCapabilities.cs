using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public sealed record DispatchingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload>
(
  DispatchingCapabilities Dispatching,
  SchedulingCapabilities<TKey, TPayload> Scheduling,
  AbandoningCapabilities<TKey, TPayload> Abandoning,
  ClosingCapabilities<TKey, TPayload> Closing
);

