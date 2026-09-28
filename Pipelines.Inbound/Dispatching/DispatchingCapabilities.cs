using Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

public sealed record DispatchingCapabilities
(
  Operations.Inbound.DeadLetterEnvelope.DispatchingCapabilities Dispatching,
  SchedulingCapabilities Scheduling,
  AbandoningCapabilities Abandoning,
  ClosingCapabilities Closing
);

