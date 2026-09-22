using DeadLetter = Operations.Inbound.DeadLetter;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

internal readonly union DispatchingSignal(
  DispatchingEntries,
  DeadLetterEnvelope.DispatchingStates,
  DeadLetter.SchedulingStates,
  DeadLetter.AbandoningStates,
  DeadLetter.ClosingStates
);
