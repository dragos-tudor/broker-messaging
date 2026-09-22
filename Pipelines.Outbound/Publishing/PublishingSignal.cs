using Outbox = Operations.Outbound.Outbox;
using Envelope = Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

internal readonly union PublishingSignal(
  PublishingEntries,
  Outbox.MappingStates,
  Envelope.PublishingStates,
  Envelope.ProducingStates,
  Outbox.SchedulingStates,
  Outbox.AbandoningStates,
  Outbox.ClosingStates
);
