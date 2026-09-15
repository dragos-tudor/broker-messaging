using DeadLetter = Operations.Inbound.DeadLetter;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

internal enum PublishingEntry { Start }

internal readonly union PublishingInput(
  PublishingEntry,
  DeadLetter.MappingStates,
  DeadLetterEnvelope.PublishingStates,
  DeadLetterEnvelope.ProducingStates,
  DeadLetter.SchedulingStates,
  DeadLetter.AbandoningStates,
  DeadLetter.ClosingStates
);
