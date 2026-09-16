using DeadLetter = Operations.Inbound.DeadLetter;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

internal enum DispatchingEntry { Start }

internal readonly union DispatchingSignal(
  DispatchingEntry,
  DeadLetterEnvelope.DispatchingStates,
  DeadLetter.SchedulingStates,
  DeadLetter.AbandoningStates,
  DeadLetter.ClosingStates
);
