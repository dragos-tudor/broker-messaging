
using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

public sealed record DispatchingCapabilities(
  Operations.Outbound.Envelope.DispatchingCapabilities Dispatching,
  SchedulingCapabilities Scheduling,
  AbandoningCapabilities Abandoning,
  ClosingCapabilities Closing
);
