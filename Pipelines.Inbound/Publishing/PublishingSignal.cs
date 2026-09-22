using DeadLetter = Operations.Inbound.DeadLetter;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

internal readonly union PublishingSignal(
  PublishingEntries,
  DeadLetter.MappingStates,
  DeadLetterEnvelope.PublishingStates,
  DeadLetterEnvelope.ProducingStates,
  DeadLetter.SchedulingStates,
  DeadLetter.AbandoningStates,
  DeadLetter.ClosingStates
);
