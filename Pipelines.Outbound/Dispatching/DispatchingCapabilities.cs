using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

public sealed record DispatchingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload>(
  Operations.Outbound.Envelope.DispatchingCapabilities Dispatching,
  SchedulingCapabilities<TKey, TPayload> Scheduling,
  AbandoningCapabilities<TKey, TPayload> Abandoning,
  ClosingCapabilities<TKey, TPayload> Closing
);
