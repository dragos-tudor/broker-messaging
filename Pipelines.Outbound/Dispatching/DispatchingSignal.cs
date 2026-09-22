using Outbox = Operations.Outbound.Outbox;
using Envelope = Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

internal readonly union DispatchingSignal(
  DispatchingEntries,
  Envelope.DispatchingStates,
  Outbox.SchedulingStates,
  Outbox.AbandoningStates,
  Outbox.ClosingStates
);
