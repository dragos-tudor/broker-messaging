using Outbox = Operations.Outbound.Outbox;
using Envelope = Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

internal enum DispatchingEntry { Start }

internal readonly union DispatchingInput(
  DispatchingEntry,
  Envelope.DispatchingStates,
  Outbox.SchedulingStates,
  Outbox.AbandoningStates,
  Outbox.ClosingStates
);
