using Outbox = Operations.Outbound.Outbox;
using Envelope = Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

internal enum PublishingEntry { Start }

internal readonly union PublishingSignal(
  PublishingEntry,
  Outbox.MappingStates,
  Envelope.PublishingStates,
  Envelope.ProducingStates,
  Outbox.SchedulingStates,
  Outbox.AbandoningStates,
  Outbox.ClosingStates
);
